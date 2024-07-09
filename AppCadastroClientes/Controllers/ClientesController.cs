using AppCadastroClientes.Helpers;
using AppCadastroClientes.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AppCadastroClientes.Controllers
{
    public class ClientesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index(int? pagina)
        {
            var ListaClientes = db.Clientes.AsQueryable().Where(c => !c.IsDeleted).OrderBy(x => x.Id);
            int paginaTamanho = 50;
            int paginaNumero = (pagina ?? 1);

            return View(ListaClientes.ToPagedList(paginaNumero, paginaTamanho));
        }

        public ActionResult FiltrarClientes(bool TodosAtivos, bool TodosInativos, string nome, string documento, int? pagina)
        {
            int paginaTamanho = 50;
            int paginaNumero = (pagina ?? 1);
            var ListaClientes = db.Clientes.AsQueryable();

            if (!(TodosAtivos && TodosInativos))
                ListaClientes = ListaClientes.Where(c => (TodosAtivos ? !c.IsDeleted : c.IsDeleted));

            if (!string.IsNullOrEmpty(nome))
                ListaClientes = ListaClientes.Where(c => c.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(documento))
                ListaClientes = ListaClientes.Where(c => c.Documento.Contains(documento));

            return PartialView("_ClientesTable", ListaClientes.OrderBy(x => x.Id).ToPagedList(paginaNumero, paginaTamanho));
        }

        public ActionResult Create()
        {
            return View();
        }

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
