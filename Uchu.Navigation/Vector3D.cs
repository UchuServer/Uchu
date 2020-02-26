using System;

namespace Uchu.Navigation
{
	/// <summary>
	/// Basic geometry class : easy to replace
	/// Written so as to be generalized
	/// </summary>
	public class Vector3D
	{
		private readonly double[] _coordinates = new double[3];

		/// <summary>
		/// Vector3D constructor.
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument array must not be null.</exception>
		/// <exception cref="ArgumentException">The Coordinates' array must contain exactly 3 elements.</exception>
		/// <param name="coordinates">An array containing the three coordinates' values.</param>
		public Vector3D(double[] coordinates)
		{
			if ( coordinates == null ) throw new ArgumentNullException();
			if ( coordinates.Length!=3 ) throw new ArgumentException("The Coordinates' array must contain exactly 3 elements.");
			Dx = coordinates[0]; Dy = coordinates[1]; Dz = coordinates[2];
		}

		/// <summary>
		/// Vector3D constructor.
		/// </summary>
		/// <param name="deltaX">DX coordinate.</param>
		/// <param name="deltaY">DY coordinate.</param>
		/// <param name="deltaZ">DZ coordinate.</param>
		public Vector3D(double deltaX, double deltaY, double deltaZ)
		{
			Dx = deltaX; Dy = deltaY; Dz = deltaZ;
		}

		/// <summary>
		/// Constructs a Vector3D with two points.
		/// </summary>
		/// <param name="p1">First point of the vector.</param>
		/// <param name="p2">Second point of the vector.</param>
		public Vector3D(Point3D p1, Point3D p2)
		{
			Dx = p2.X-p1.X; Dy = p2.Y-p1.Y; Dz = p2.Z-p1.Z;
		}

		/// <summary>
		/// Accede to coordinates by indexes.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">Illegal value for CoordinateIndex.</exception>
		public double this[int coordinateIndex]
		{
			get => _coordinates[coordinateIndex];
			set => _coordinates[coordinateIndex] = value;
		}

		/// <summary>
		/// Gets/Sets delta X value.
		/// </summary>
		public double Dx { set => _coordinates[0] = value;
			get => _coordinates[0];
		}

		/// <summary>
		/// Gets/Sets delta Y value.
		/// </summary>
		public double Dy { set => _coordinates[1] = value;
			get => _coordinates[1];
		}

		/// <summary>
		/// Gets/Sets delta Z value.
		/// </summary>
		public double Dz { set => _coordinates[2] = value;
			get => _coordinates[2];
		}

		/// <summary>
		/// Multiplication of a vector by a scalar value.
		/// </summary>
		/// <param name="v">Vector to operate.</param>
		/// <param name="factor">Factor value.</param>
		/// <returns>New vector resulting from the multiplication.</returns>
		public static Vector3D operator*(Vector3D v, double factor)
		{
			var @new = new double[3];
			for(var i=0; i<3; i++) @new[i] = v[i]*factor;
			return new Vector3D(@new);
		}

		/// <summary>
		/// Division of a vector by a scalar value.
		/// </summary>
		/// <exception cref="ArgumentException">Divider cannot be 0.</exception>
		/// <param name="v">Vector to operate.</param>
		/// <param name="divider">Divider value.</param>
		/// <returns>New vector resulting from the division.</returns>
		public static Vector3D operator/(Vector3D v, double divider)
		{
			if ( divider==0 ) throw new ArgumentException("Divider cannot be 0 !\n");
			var @new = new double[3];
			for(var i=0; i<3; i++) @new[i] = v[i]/divider;
			return new Vector3D(@new);
		}

		/// <summary>
		/// Gets the square norm of the vector.
		/// </summary>
		public double SquareNorm
		{
			get
			{
				double sum = 0;
				for (var i=0; i<3; i++) sum += _coordinates[i]*_coordinates[i];
				return sum;
			}
		}

		/// <summary>
		/// Gets the norm of the vector.
		/// </summary>
		/// <exception cref="InvalidOperationException">Vector's norm cannot be changed if it is 0.</exception>
		public double Norm
		{
			get => Math.Sqrt(SquareNorm);
			set
			{
				var n = Norm;
				if ( n==0 ) throw new InvalidOperationException("Cannot set norm for a nul vector !");
				if ( n!=value )
				{
					var facteur = value/n;
					for (var i=0; i<3; i++) this[i]*=facteur;
				}
			}
		}

		/// <summary>
		/// Scalar product between two vectors.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>Value resulting from the scalar product.</returns>
		public static double operator|(Vector3D v1, Vector3D v2)
		{
			double scalarProduct = 0;
			for(var i=0; i<3; i++) scalarProduct += v1[i]*v2[i];
			return scalarProduct;
		}

		/// <summary>
		/// Returns a point resulting from the translation of a specified point.
		/// </summary>
		/// <param name="p">Point to translate.</param>
		/// <param name="v">Vector to apply for the translation.</param>
		/// <returns>Point resulting from the translation.</returns>
		public static Point3D operator+(Point3D p, Vector3D v)
		{
			var @new = new double[3];
			for(var i=0; i<3; i++) @new[i] = p[i]+v[i];
			return new Point3D(@new);
		}
	}
}
