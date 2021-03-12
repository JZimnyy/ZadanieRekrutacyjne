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

        public ActionResult Treeview(int parentID)
        {
            ViewBag.parentID = parentID;
            return PartialView(db.Trees.ToList());
        }

        // GET: Tree/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: Tree/Create
        public ActionResult Create()
        {
            var node = db.Trees;

            ViewBag.Nodes = node;

            return View();
        }

        // POST: Tree/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TreeId,Name,ParentID")] Tree tree)
        {
            if (ModelState.IsValid)
            {
                db.Trees.Add(tree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tree);
        }

        // GET: Tree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tree tree = db.Trees.Find(id);
            if (tree == null)
            {
                return HttpNotFound();
            }

            var node = db.Trees.Where(a=>a.TreeId!=id);
            ViewBag.Nodes = node;
            return View(tree);
        }

        // POST: Tree/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TreeId,Name,ParentID")] Tree tree)
        {
            if (tree.TreeId == tree.ParentID)
                return View(tree);

            if (ModelState.IsValid)
            {
                db.Entry(tree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tree);
        }

        // GET: Tree/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: Tree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //usuwanie tylko wybranego węzła, dzieci tego węzła zostają przeniesione o poziom w górę

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

            foreach(Tree t in toModify)
            {
                t.ParentID = parentNodeID;
                db.Entry(t).State = EntityState.Modified;
            }

            db.Trees.Remove(tree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll(int? id)
        {
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

        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAllConfirmed(int id)
        {
            //funkcja służąca do usuwania węzła razem z dziećmi

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
