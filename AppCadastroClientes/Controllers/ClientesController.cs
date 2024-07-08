using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppCadastroClientes.Helpers;
using AppCadastroClientes.Models;

namespace AppCadastroClientes.Controllers
{
    public class ClientesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Clientes
        public ActionResult Index()
        {
            return View(db.Clientes.AsQueryable().Where(c => !c.IsDeleted));
        }

        public ActionResult FiltrarClientes(bool somenteAtivos, string nome, string documento)
        {
            var clientes = db.Clientes.AsQueryable();

            if (somenteAtivos)
            {
                clientes = clientes.Where(c => !c.IsDeleted);
            }

            if (!string.IsNullOrEmpty(nome))
            {
                clientes = clientes.Where(c => c.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(documento))
            {
                clientes = clientes.Where(c => c.Documento.Contains(documento));
            }

            return PartialView("_ClientesTable", clientes.ToList());
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Tipo,Documento,DataCadastro,Telefone,IsDeleted")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                if (!ValidarDocumento(cliente))
                {
                    return View(cliente); // Retorna a view com o estado do modelo inválido
                }

                cliente.DataCadastro = DateTime.Now;
                cliente.IsDeleted = false;
                db.Clientes.Add(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Tipo,Documento,DataCadastro,Telefone,IsDeleted")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                if (!ValidarDocumento(cliente))
                {
                    return View(cliente); // Retorna a view com o estado do modelo inválido
                }

                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Clientes.Find(id);
            cliente.IsDeleted = true;
            db.Entry(cliente).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool ValidarDocumento(Cliente cliente)
        {
            bool isDocumentoValido;
            string documentoFormatado;

            if (cliente.Tipo == "PF")
            {
                isDocumentoValido = DocumentoHelper.ValidarCpf(cliente.Documento);
                documentoFormatado = DocumentoHelper.FormatDocumento(cliente.Documento, "PF");
            }
            else if (cliente.Tipo == "PJ")
            {
                isDocumentoValido = DocumentoHelper.ValidarCnpj(cliente.Documento);
                documentoFormatado = DocumentoHelper.FormatDocumento(cliente.Documento, "PJ");
            }
            else
            {
                ModelState.AddModelError("", "Tipo de documento inválido.");
                return false;
            }

            if (!isDocumentoValido)
            {
                ModelState.AddModelError("", "Documento inválido.");
                return false;
            }

            cliente.Documento = documentoFormatado;
            return true;
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
