using Microsoft.AspNetCore.Mvc;
using NetCoreHighCharts.Models;
using NetCoreHighCharts.Repositories;

namespace NetCoreHighCharts.Controllers
{
    public class ArticuloController : Controller
    {
        private RepositoryArticulos repo;

        public ArticuloController(RepositoryArticulos repo)
        {
            this.repo = repo;
        }
        public IActionResult InfoArticulos()
        {
            return View(this.repo.GetAllArticulos());
        }

        public IActionResult _PaginacionAjax(int? posicion)
        {
            int numarticulos = 0;
            ArticuloXML articulo = this.repo.GetArticuloXPosicion
                (posicion.Value, ref numarticulos);

            ViewData["DATOS"] = "Articulo " + (posicion + 1) + " de " + numarticulos;

            int siguiente = posicion.Value + 1;
            if (siguiente >= numarticulos)
            {
                siguiente = 0;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 0)
            {
                anterior = numarticulos - 1;
            }
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;

            return PartialView("_PaginacionAjax", articulo);
        }

        public IActionResult EditArticulo(int idarticulo)
        {
            return View(this.repo.GetArticuloById(idarticulo));
        }

        public async Task<IActionResult> DeleteArticulo(int idarticulo)
        {
            await this.repo.DeleteArticulo(idarticulo);
            return RedirectToAction("ListaArticulos", "Articulo");
        }

        [HttpPost]
        public async Task<IActionResult> EditArticulo(ArticuloXML articulo)
        {
            await this.repo.UpdateArticulo(articulo);
            return RedirectToAction("ListaArticulos", "Articulo");
        }

        public IActionResult InsertArticulo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertArticulo(string nombre, string descripcion, int calorias, int proteinas, int hidratos, int glucosa)
        {
            await this.repo.InsertArticulos(nombre,descripcion,calorias,proteinas,hidratos,glucosa);
            return RedirectToAction("ListaArticulos", "Articulo");
        }

        public IActionResult ListaArticulos()
        {
            return View(this.repo.GetAllArticulos());
        }
    }
}
