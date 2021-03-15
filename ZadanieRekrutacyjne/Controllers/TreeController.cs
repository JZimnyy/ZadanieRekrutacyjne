using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZadanieRekrutacyjne.Models;

namespace ZadanieRekrutacyjne.Controllers
{
    public class TreeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tree
        public ActionResult Index()
        {
            return View(db.Trees.ToList());
        }

        public ActionResult Treeview(int? parentID, int? sortType)
        {
            //funkcja, służaca do wyświetlania drzewa
            //funkcja ta jako parametry przyjmuje ID rodzica oraz wartość służącą do zidentyfikowania rodzaju sortowania

            if(parentID == null)
            {
                //jeżeli nasz parametr parentID jest pusty przypisujemy mu wartość 0
                //wartość 0 jest rodzicem dla korzenia drzewa
                parentID = 0;
            }

            //wartość zmiennej parentID przekazujemy do widoku, a następnie generujemy listę obiektów klasy Tree z bazy danych(posortowanych w odpowiedni sposób) 
            //na końcu zwracamy widok częściowy
            ViewBag.parentID = parentID;
            var lista = db.Trees.ToList();

            if (sortType == 1)
            {
                var listaAZ = db.Trees.OrderBy(x => x.Name).ToList();
                return PartialView(listaAZ);
            }
            else if (sortType == 2)
            {
                var listaZA = db.Trees.OrderByDescending(x => x.Name).ToList();
                return PartialView(listaZA);
            }
            
            

            return PartialView(lista);
        }

        

        

        [Authorize(Roles = "Admin")]
        public ActionResult AddChild(int id)
        {
            //funkcja, która służy do dodania dziecka do wybranego elementu drzewa

            //sprawdzamy czy w bazie widnieje element o danym ID
            //jeżeli nie istnieje zwracamy BadRequest
            Tree tree = db.Trees.Find(id);
            if(tree==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //w przeciwnym wypadku do modelu przesyłamy nazwę węzła do którego będziemy dodawać dziecko
            ViewBag.Rodzic = tree.Name;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddChild(int id,Tree tree)
        {
            //do modelu uzyskanego z formularza dodajemy id rodzica - jest ono zawarte w adresie
            tree.ParentID = id;

            //sprawdzamy czy model jest prawidłowy, jeżeli tak dodajemy go do bazy i wracamy do strony Tree/Index
            if(ModelState.IsValid)
            {
                db.Trees.Add(tree);
                db.SaveChanges();
                return RedirectToAction("Index", "Tree");
            }

            //w przeciwnym razie zwracamy widok formularza do ponownego uzupełnienia
            return View();
        }

        // GET: Tree/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            //funckja służaca do zmiany nazwy 

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tree tree = db.Trees.Find(id);
            if (tree == null)
            {
                return HttpNotFound();
            }

            return View(tree);
        }

        // POST: Tree/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TreeId,Name,ParentID")] Tree tree)
        {
            //najpierw sprawdzamy czy ID elementu nie jest takie samo jak ID rodzica
            //taka sytuacja nie może mieć miejsca, więc w przypadku jej wystąpinia wracamy do widoku formularza
            if (tree.TreeId == tree.ParentID)
                return View(tree);

            //jeżeli model jest w porządku zapisujemy zmiany i wracamy do strony Tree/Index
            if (ModelState.IsValid)
            {
                db.Entry(tree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //w przeciwnym razie wracamy do widoku formularza
            return View(tree);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Move(int? id)
        {
            //funkcja służaca do przenoszenia całej gałęzi

            //najpierw sprawdzamy czy użytkownik nie próbuje przenieść korzenia drzewa
            //takie działanie jest niemożliwe do wykonania, więc w przypadku jego wystąpienia wracamy do strony Tree/Index
            if(id!=1)
            {
                //sprawdzamy czy id jest puste oraz czy istnieje w bazie element o danym id
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Tree tree = db.Trees.Find(id);
                if (tree == null)
                {
                    return HttpNotFound();
                }


                //następnie przekazujemy do modelu listę wszystkich dostępnych gałęzi drzewa oraz nazwę obecnego rodzica
                var node = db.Trees.Select(x => new SelectListItem
                {
                    Value = x.TreeId.ToString(),
                    Text = x.Name
                });

                Tree parent = db.Trees.Find(tree.ParentID);

                ViewBag.CurrentParent = parent.Name;
                ViewBag.ParentID = new SelectList(node, "Value", "Text");
                return View(tree);
            }
            return RedirectToAction("Index", "Tree");
        }

        // POST: Tree/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Move([Bind(Include = "TreeId,Name,ParentID")] Tree tree)
        {
            //najpierw sprawdzamy czy ID elementu nie jest takie samo jak ID rodzica
            //taka sytuacja nie może mieć miejsca, więc w przypadku jej wystąpinia wracamy do widoku formularza
            if (tree.TreeId == tree.ParentID)
                return View(tree);

            //jeżeli model jest prawidłowy zapisujemy zmiany w bazie i wracamy do Tree/Index
            if (ModelState.IsValid)
            {
                db.Entry(tree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tree);
        }


        // GET: Tree/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            //funkcja służaca do usunięcia elementu i przeniesienia jego dzieci o poziom wyżej w hierarchii
            //sprawdzamy czy id jest puste
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //korzenia drzewa nie możemy usunąć, więc w przypadku takiego requesta wracamy do widoku Tree/Index
            if(id==1)
            {
                return RedirectToAction("Index", "Tree");
            }

            //sprawdzamy czy w bazie występuje element o danym ID
            Tree tree = db.Trees.Find(id);
            if (tree == null)
            {
                return HttpNotFound();
            }

            //do widoku przekazujemy nazwę rodzica
            Tree parent = db.Trees.Find(tree.ParentID);
            ViewBag.Rodzic = parent.Name;

            return View(tree);
        }

        // POST: Tree/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //usuwanie tylko wybranego węzła, dzieci tego węzła zostają przeniesione o poziom w górę

            if(id==1)
            {
                //jeżeli id=1 to wracamy do widoku index, poniewaź nie można usunąć korzenia
                return RedirectToAction("Index", "Tree");
            }

            //z bazy pobieramy element o danym id i tworzymy pustę liste obiektów klasy Tree
            //w tej liście będziemy zapisywać dzieci tego elementu
            //zrobimy to ponieważ musimy im zmienić id rodzica na id rodzica usuwanego elementu
            Tree tree = db.Trees.Find(id);
            List<Tree> toModify = new List<Tree>();

            var nodes = db.Trees.Where(n => n.ParentID == id);

            //jeżeli baza zwraca nam wynik przystępujemy do edycji listy elementów do usunięcia 
            if (nodes != null)
            {

                foreach (var testNode in nodes)
                {
                    //pobieramy z bazy element o danym ID i jeżeli jeszcze nie występuje na liście dodajemy go
                    Tree t = db.Trees.Find(testNode.TreeId);
                    toModify.Add(t);
                }
            }

            
            int parentNodeID = tree.ParentID;

            //w każdym obiekcie z listy toModify zmieniamy wartość pola ParentID
            foreach(Tree t in toModify)
            {
                t.ParentID = parentNodeID;
                db.Entry(t).State = EntityState.Modified;
            }

            //usuwamy element i zapisujemy zmiany
            db.Trees.Remove(tree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAll(int? id)
        {
            //funkcja, służąca do usuwania elementu razem z dziećmi
            
            //ostrzeżenie o wyczyszczeniu całego drzewa 
            if(id==1)
            {
                ViewBag.Warning = "Zamierzasz wyczyścić całe drzewo";
                ViewBag.Rodzic = "Ten element jest korzeniem drzewa";
            }

            //sprawdzenie czy id jest puste i czy baza zawiera element o danym id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tree tree = db.Trees.Find(id);
            if (tree == null)
            {
                return HttpNotFound();
            }

            //do widoku przekazujemy również nazwę rodzica
            if (id != 1)
            {
                Tree parent = db.Trees.Find(tree.ParentID);
                ViewBag.Rodzic = parent.Name;
            }
            return View(tree);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAllConfirmed(int id)
        {
            //funkcja służąca do usuwania węzła razem z dziećmi

            //jeżeli id==1 (jest to id korzenia) to czyścimy całe drzewo zostawiając jedynie korzeń
            if(id==1)
            {
                //pobieramy wszystkie elementy z bazy z wyłączeniem korzenia(czyli elementu o id=1)
                var nodes = db.Trees.Where(n => n.TreeId != 1);
                List<Tree> toDeleteAll = new List<Tree>();

                //za pomocą pętli foreach dodajemy te elementy do wcześniej utworzonej listy
                foreach(var treeNode in nodes)
                {
                    Tree t = db.Trees.Find(treeNode.TreeId);
                    toDeleteAll.Add(t);
                }

                //usuwamy element z listy, zapisujemy zmiany i wracamy do Tree/Index
                foreach(Tree t in toDeleteAll)
                {
                    db.Trees.Remove(t);
                }

                db.SaveChanges();
                return RedirectToAction("Index", "Tree");
            }

            bool contains = true;
            Tree tree = db.Trees.Find(id);
            //tworzymy listę obiektów do usunięcia i dodajemy do niej wybrany węzeł
            List<Tree> toDelete = new List<Tree>();
            toDelete.Add(tree);

            //tworzymy również listę, która zawiera ID elementów do usunięcia, ta lista posłuży nam do wyszukiwania dzieci tych elementów
            //pierwszym elementem tej listy jest ID węzła przeznaczonego do usunięcia
            List<int> parentIDs = new List<int>();
            parentIDs.Add(id);

            //aby przeszukać całą bazę wykorzystuję pętle do while
            do
            {
                //za pomocą pętli foreach sprawdzamy czy w bazie występują węzły, które jako ParentID posiadają element zawarty w naszej liście
                foreach(int i in parentIDs)
                {
                    var nodes = db.Trees.Where(n => n.ParentID == i);

                    //jeżeli baza zwraca nam wynik przystępujemy do edycji listy elementów do usunięcia 
                    if (nodes != null)
                    {

                        foreach (var testNode in nodes)
                        {
                            //pobieramy z bazy element o danym ID i jeżeli jeszcze nie występuje na liście dodajemy go
                            Tree t = db.Trees.Find(testNode.TreeId);
                            if(!(toDelete.Contains(t)))
                                toDelete.Add(t);
                        }
                    }
                        
                     
                }

                //następnie przechodzimy do edycji listy, która zawiera ID elementów przeznaczonych do usunięcia
                foreach(Tree t in toDelete)
                {
                    //na początku zmieniamy wartosc contains na false
                    contains = false;

                    //następnie sprawdzamy czy obie listy zawierają te same dane(tzn czy id wszystkich elementow przeznaczonych do usuniecia znajduje sie na liscie int-ów)
                    if(!(parentIDs.Contains(t.TreeId)))
                    {
                        //jeżeli znajdziemy ID, które nie występuje na liście, zmieniamy constains na true i dodajemy ten ID do listy
                        //dzięki zmianie wartosci contains na true, pętla wykona kolejną iteracje w poszukiwaniu kolejnych dzieci
                        contains = true;
                        parentIDs.Add(t.TreeId);
                    }
                             
                    //jeżeli instrukcja warunkowa zwróci wartość false pętla do while zakończy swoje działanie
                }


            } while (contains == true);

            //za pomocą foreach po kolei usuwamy elementy
            foreach(Tree t in toDelete)
            {
                db.Trees.Remove(t);
            }

            //zapisujemy zmiany i przechodzimy do akcji index
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
