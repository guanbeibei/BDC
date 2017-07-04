using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace NCZJDDJFZ.Tools
{
    public sealed class HelperFunctions
    {
        ///<summary>
        ///Converts the coordinate system of point from UCS to WCS
        ///<summary>
        ///<param name = "sourcePoint">[in] The source point</param>
        ///<returns>Returns the point after transformed.</returns>
        public static Point3d Ucs2Wcs(Point3d sourcePoint)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d ucsMatrix = ed.CurrentUserCoordinateSystem;
            Matrix3d wcsMatrix = ucsMatrix.Inverse();
            Point3d transformedPoint = sourcePoint.TransformBy(wcsMatrix);
            return transformedPoint;
        }

        ///<summary>
        ///Selects the objects
        ///<summary>
        ///<param name = "objIds">[out] An object collection which holds the selected objects</param>
        ///<returns>Returns the status of selection</returns>
        public static ObjectIdCollection SelectIds(SelectionFilter filter)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            ObjectIdCollection objIds = new ObjectIdCollection();
            PromptSelectionResult promptSelectionResult = ed.SelectAll(filter);
            SelectionSet selectionSet = promptSelectionResult.Value;
            if (selectionSet == null)
            {
                return objIds;
            }
            foreach (SelectedObject selObj in selectionSet)
            {
                objIds.Add(selObj.ObjectId);
            }
            return objIds;
        }

        private HelperFunctions()
        {
        }
    }
}
