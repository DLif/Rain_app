using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RainMan.DataModels;

namespace MobileServiceFinal.Models
{
    public class Polygon
    {
         public int size { get; set; }  //how many points the polygon has
        public List<PixelRep> Pixels { get; set; }  //the countour
        public Polygon(int size, List<PixelRep> pixels)
        {
            this.size = size;
            this.Pixels = pixels;
        }


    }

    class PolygonPixels
    {
        
        static int Picture_x_size = 512;
        static int Picture_y_size = 512;

        public static List<PixelRep> getAllPointsInsidePolygon(Polygon p)
        {
            List<PixelRep> result  = new  List<PixelRep>();
            for (int i =0; i< Picture_x_size; i++)
            {
                 for(int j =0; j< Picture_y_size; j++)
                 {
                     if (isPointInsidePolygon( p, i, j))
                     {
                         result.Add(new PixelRep(i,j));
                     }
                     

                 }
            }
            return result;
        }
        public static Boolean isPointInsidePolygon ( Polygon p, int x, int y)
        {

              int   i;
              int j=p.size - 1 ;
              bool  oddNodes= false;

              for (i=0; i<p.size; i++)
              {
                if ((p.Pixels[i].Y< y && p.Pixels[j].Y>=y
                ||   p.Pixels[j].Y< y && p.Pixels[i].Y>=y)
                &&  (p.Pixels[i].X<=x || p.Pixels[j].X<x)) {
                  if (p.Pixels[i].X+(y-p.Pixels[i].Y)/(p.Pixels[j].Y-p.Pixels[i].Y)*(p.Pixels[j].X-p.Pixels[i].X)<x) {
                    oddNodes=!oddNodes; }}
                j=i;
              }

              return oddNodes; 

        }

    }


}