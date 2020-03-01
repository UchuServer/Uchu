
using System;

namespace Uchu.Navigation
{
	/// <summary>
	/// Basic geometry class : easy to replace
	/// Written so as to be generalized
	/// </summary>
	[Serializable]
	public class Point3D
	{
		private double[] _coordinates = new double[3];

		/// <summary>
		/// Point3D constructor.
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument array must not be null.</exception>
		/// <exception cref="ArgumentException">The Coordinates' array must contain exactly 3 elements.</exception>
		/// <param name="coordinates">An array containing the three coordinates' values.</param>
		public Point3D(double[] coordinates)
		{
			if ( coordinates == null ) throw new ArgumentNullException();
			if ( coordinates.Length!=3 ) throw new ArgumentException("The Coordinates' array must contain exactly 3 elements.");
			X = coordinates[0]; Y = coordinates[1]; Z = coordinates[2];
		}

		/// <summary>
		/// Point3D constructor.
		/// </summary>
		/// <param name="coordinateX">X coordinate.</param>
		/// <param name="coordinateY">Y coordinate.</param>
		/// <param name="coordinateZ">Z coordinate.</param>
		public Point3D(double coordinateX, double coordinateY, double coordinateZ)
		{
			X = coordinateX; Y = coordinateY; Z = coordinateZ;
		}

		/// <summary>
		/// Accede to coordinates by indexes.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">Index must belong to [0;2].</exception>
		public double this[int coordinateIndex]
		{
			get => _coordinates[coordinateIndex];
			set => _coordinates[coordinateIndex] = value;
		}

		/// <summary>
		/// Gets/Set X coordinate.
		/// </summary>
		public double X { set => _coordinates[0] = value;
			get => _coordinates[0];
		}

		/// <summary>
		/// Gets/Set Y coordinate.
		/// </summary>
		public double Y { set => _coordinates[1] = value;
			get => _coordinates[1];
		}

		/// <summary>
		/// Gets/Set Z coordinate.
		/// </summary>
		public double Z { set => _coordinates[2] = value;
			get => _coordinates[2];
		}

		/// <summary>
		/// Returns the distance between two points.
		/// </summary>
		/// <param name="p1">First point.</param>
		/// <param name="p2">Second point.</param>
		/// <returns>Distance value.</returns>
		public static double DistanceBetween(Point3D p1, Point3D p2)
		{
			return Math.Sqrt((p1.X-p2.X)*(p1.X-p2.X)+(p1.Y-p2.Y)*(p1.Y-p2.Y));
		}

		/// <summary>
		/// Returns the projection of a point on the line defined with two other points.
		/// When the projection is out of the segment, then the closest extremity is returned.
		/// </summary>
		/// <exception cref="ArgumentNullException">None of the arguments can be null.</exception>
		/// <exception cref="ArgumentException">P1 and P2 must be different.</exception>
		/// <param name="pt">Point to project.</param>
		/// <param name="p1">First point of the line.</param>
		/// <param name="p2">Second point of the line.</param>
		/// <returns>The projected point if it is on the segment / The closest extremity otherwise.</returns>
		public static Point3D ProjectOnLine(Point3D pt, Point3D p1, Point3D p2)
		{
			if ( pt==null || p1==null || p2==null ) throw new ArgumentNullException("None of the arguments can be null.");
			if ( p1.Equals(p2) ) throw new ArgumentException("P1 and P2 must be different.");
			var vLine = new Vector3D(p1, p2);
			var v1Pt = new Vector3D(p1, pt);
			var translation = vLine*(vLine|v1Pt)/vLine.SquareNorm;
			var projection = p1+translation;

			var v1Pjt = new Vector3D(p1, projection);
			var d1 = v1Pjt|vLine;
			if ( d1<0 ) return p1;

			var v2Pjt = new Vector3D(p2, projection);
			var d2 = v2Pjt|vLine;
			if ( d2>0 ) return p2;

			return projection;
		}

		/// <summary>
		/// Object.Equals override.
		/// Tells if two points are equal by comparing coordinates.
		/// </summary>
		/// <exception cref="ArgumentException">Cannot compare Point3D with another type.</exception>
		/// <param name="point">The other 3DPoint to compare with.</param>
		/// <returns>'true' if points are equal.</returns>
		public override bool Equals(object point)
		{
			var p = (Point3D)point;
			if ( p==null ) throw new ArgumentException("Object must be of type "+GetType());
			var resultat = true;
			for (var i=0; i<3; i++) resultat &= p[i].Equals(this[i]);
			return resultat;
		}

		/// <summary>
		/// Object.GetHashCode override.
		/// </summary>
		/// <returns>HashCode value.</returns>
		public override int GetHashCode()
		{
			double hashCode = 0;
			for (var i=0; i<3; i++) hashCode += this[i];
			return (int)hashCode;
		}

		/// <summary>
		/// Object.GetHashCode override.
		/// Returns a textual description of the point.
		/// </summary>
		/// <returns>String describing this point.</returns>
		public override string ToString()
		{
			var deb = "{";
			var sep = ";";
			var fin = "}";
			var resultat = deb;
			var dimension = 3;
			for (var i=0; i<dimension; i++)
				resultat += _coordinates[i] + (i!=dimension-1 ? sep : fin);
			return resultat;
		}
	}
}
