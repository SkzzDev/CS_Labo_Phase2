using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyCartographyObjects;
using ZZUtils;

namespace MyCartography
{
    class Program
    {
        static void Main()
        {
            List<Coordonnees> squareCoords = new List<Coordonnees>{
                new Coordonnees (0, 0),
                new Coordonnees (0, 10),
                new Coordonnees (10, 10),
                new Coordonnees (10, 0)
            };
            List<Coordonnees> rectCoords = new List<Coordonnees>{
                new Coordonnees (0, 0),
                new Coordonnees (25, 0),
                new Coordonnees (25, 10),
                new Coordonnees (0, 10)
            };
            List<Coordonnees> rand1Coords = new List<Coordonnees>{
                new Coordonnees (0, 0),
                new Coordonnees (20, 0),
                new Coordonnees (25, 10),
                new Coordonnees (0, 10)
            };
            List<Coordonnees> rand2Coords = new List<Coordonnees>{
                new Coordonnees (0, 0),
                new Coordonnees (5, 5),
                new Coordonnees (5, 2),
                new Coordonnees (3, 1)
            };
            List<Coordonnees> rand3Coords = new List<Coordonnees>{
                new Coordonnees (0, 0),
                new Coordonnees (0, 15),
                new Coordonnees (5, 10),
                new Coordonnees (5, 0),
                new Coordonnees (9, 9)
            };
            Polyline pl1 = new Polyline(squareCoords), pl2 = new Polyline(pl1);
            Polygon pg1 = new Polygon(pl1), pg2 = new Polygon(rand3Coords);
            Coordonnees coord1 = new Coordonnees(12, 7), coord2 = new Coordonnees();
            POI poi1 = new POI(), poi2 = new POI(7, 9, "Second one");

            List<CartoObj> cartoObjs = new List<CartoObj> { pl1, pl2, pg1, pg2, coord1, coord2, poi1, poi2 };

            Console.WriteLine("All elements:\n");

            foreach (CartoObj cartoObj in cartoObjs) {
                Console.WriteLine(cartoObj + "\n");
            }

            Console.WriteLine("\n-----------------------------------------------\nIPointy elements:\n");

            foreach (CartoObj cartoObj in cartoObjs) {
                if (cartoObj is IPointy) {
                    cartoObj.Draw();
                }
            }

            Console.WriteLine("\n-----------------------------------------------\nNot IPointy elements:\n");

            foreach (CartoObj cartoObj in cartoObjs) {
                if (!(cartoObj is IPointy)) {
                    cartoObj.Draw();
                }
            }

            Console.WriteLine("\n-----------------------------------------------\nThe 5 polylines:\n");

            List<Polyline> polylines = new List<Polyline> {
                pl1, pl2,
                new Polyline(rand1Coords),
                new Polyline(rand2Coords),
                new Polyline(rand3Coords)
            };

            foreach (Polyline polyline in polylines) {
                polyline.Draw();
            }

            Console.WriteLine("\n-----------------------------------------------\nSorted polylines (by perimeter):\n");

            polylines.Sort();

            foreach (Polyline polyline in polylines) {
                Console.WriteLine(polyline.GetPerimeter().ToString("0.00") + "cm => (#" + polyline.Id + ") " + polyline.GetType());
            }

            Console.WriteLine("\n-----------------------------------------------\nSorted polylines (by bouding box area):\n");

            MyPolylineBoundingBoxComparer comparer = new MyPolylineBoundingBoxComparer();
            polylines.Sort(comparer);

            foreach (Polyline polyline in polylines) {
                Console.WriteLine(polyline.GetBoundingBoxArea().ToString("0.00") + "cm^2 => (#" + polyline.Id + ") " + polyline.GetType());
            }

            Console.WriteLine("\n-----------------------------------------------\nFind same polylines:\n");

            Polyline toFind = new Polyline(), findResult = polylines.Find(x => x.GetPerimeter() == toFind.GetPerimeter());

            if (findResult != null) {
                Console.WriteLine("Polyline #{0} ({1}cm)\nHas the same perimeter as\nPolyline #{2} ({3}cm)", findResult.Id, findResult.GetPerimeter(), toFind.Id, toFind.GetPerimeter());
            } else {
                Console.WriteLine("No polylines have the same perimeter.");
            }

            Coordonnees pointToFind = new Coordonnees(2, 2);

            Console.WriteLine("\n-----------------------------------------------\nPolylines that are near the point: {0}\n", pointToFind);

            double precision = 5;

            foreach (Polyline polyline in polylines) {
                if (polyline.IsPointClose(pointToFind, precision)) {
                    Console.WriteLine("Polyline #{0} is near the point!", polyline.Id);
                }
            }

            Console.WriteLine("\n-----------------------------------------------\nSorted cartoObjs (number of Coordonnees objects contained in it):\n");

            MyCartoObjCoordonneesComparer comparer2 = new MyCartoObjCoordonneesComparer();
            cartoObjs.Sort(comparer2);

            foreach (CartoObj cartoObj in cartoObjs) {
                Console.WriteLine(cartoObj.GetCoordonneesNb() + " coordonnees => (#" + cartoObj.Id + ") " + cartoObj.GetType());
            }

            Console.ReadKey();
        }
    }
}
