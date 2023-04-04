using NetCoreHighCharts.Helpers;
using NetCoreHighCharts.Models;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace NetCoreHighCharts.Repositories
{
    public class RepositoryArticulos
    {
        private HelperPathProvider helper;
        private XDocument documentArticulos;
        private string pathArticulos;

        public RepositoryArticulos(HelperPathProvider helper)
        {
            this.helper = helper;
            this.pathArticulos = this.helper.MapPath("articulos.xml", Folders.documents);
            documentArticulos = XDocument.Load(this.pathArticulos);
        }

        public ArticuloXML GetArticuloXPosicion(int posicion, ref int numeroarticulos)
        {
            //VOY A RECUPERAR LA COLECCION DE ESCENAS DE UNA PELICULA
            //PARA ELLO, UTILIZAMOS EL METODO ANTERIOR
            List<ArticuloXML> listaarticulos = this.GetAllArticulos();
            numeroarticulos = listaarticulos.Count;
            //VAMOS A PAGINAR DE UNO EN UNO
            ArticuloXML articulo = listaarticulos.Skip(posicion).Take(1).FirstOrDefault();
            return articulo;
        }


        public List<ArticuloXML> GetAllArticulos()
        {

            var consulta = from datos in documentArticulos.Descendants("articulo")
                           select datos;

            List<ArticuloXML> listaarticulos = new List<ArticuloXML>();
            foreach (XElement tag in consulta)
            {
                ArticuloXML articulo = new ArticuloXML();
                articulo.IdArticulo = int.Parse(tag.Attribute("idarticulo").Value);
                articulo.Nombre = tag.Element("nombre").Value;
                articulo.Descripcion = tag.Element("descripcion").Value;
                articulo.Calorias = int.Parse(tag.Element("calorias").Value);
                articulo.Proteinas = int.Parse(tag.Element("proteinas").Value);
                articulo.Hidratos = int.Parse(tag.Element("hidratos").Value);
                articulo.Glucosa = int.Parse(tag.Element("glucosa").Value);

                listaarticulos.Add(articulo);
            }
            return listaarticulos;
        }

        public ArticuloXML GetArticuloById(int idarticulo)
        {
            return this.GetAllArticulos().Where(x => x.IdArticulo == idarticulo).AsEnumerable().FirstOrDefault();
        }

        public async Task InsertArticulos(string nombre, string descripcion, int calorias, int proteinas, int hidratos, int glucosa)
        {
            XElement rootArticulo = new XElement("articulo");

            rootArticulo.Add(new XAttribute("idarticulo", this.GetAllArticulos().Max(x => x.IdArticulo) + 1));
            rootArticulo.Add(new XElement("nombre", nombre));
            rootArticulo.Add(new XElement("descripcion", descripcion));
            rootArticulo.Add(new XElement("calorias", calorias));
            rootArticulo.Add(new XElement("proteinas", proteinas));
            rootArticulo.Add(new XElement("hidratos", hidratos));
            rootArticulo.Add(new XElement("glucosa", glucosa));

            this.documentArticulos.Element("articulos").Add(rootArticulo);
            this.documentArticulos.Save(this.pathArticulos);
        }

        public async Task DeleteArticulo(int idarticulo)
        {
            var consulta = from datos in
                           this.documentArticulos.Descendants("articulo")
                           where int.Parse(datos.Attribute("idarticulo").Value) == idarticulo
                           select datos;

            XElement articuloXML = consulta.FirstOrDefault();

            articuloXML.Remove();
            this.documentArticulos.Save(this.pathArticulos);
        }

        public async Task UpdateArticulo(ArticuloXML articulo)
        {
            var consulta = from datos in
                           this.documentArticulos.Descendants("articulo")
                           where int.Parse(datos.Attribute("idarticulo").Value) == articulo.IdArticulo
                           select datos;

            XElement articuloXML = consulta.FirstOrDefault();

            articuloXML.Element("nombre").Value = articulo.Nombre;
            articuloXML.Element("descripcion").Value = articulo.Descripcion;
            articuloXML.Element("calorias").Value = articulo.Calorias.ToString();
            articuloXML.Element("proteinas").Value = articulo.Proteinas.ToString();
            articuloXML.Element("hidratos").Value = articulo.Hidratos.ToString();
            articuloXML.Element("glucosa").Value = articulo.Glucosa.ToString();

            this.documentArticulos.Save(this.pathArticulos);
        }
    }
}
