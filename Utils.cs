using System;
namespace Caravaner {
    public class Utils {
        public Utils() {
        }
    }
    public struct Vector2Int {
        public readonly int x;
		public readonly int y;
		public static readonly Vector2Int Zero = new Vector2Int(0, 0);
        public static readonly Vector2Int N = new Vector2Int(0, 1);
        public static readonly Vector2Int S = new Vector2Int(0, -1);
        public static readonly Vector2Int E = new Vector2Int(1, 0);
        public static readonly Vector2Int W = new Vector2Int(-1, 0);

		public Vector2Int(int x, int y) {
			this.x = x;
			this.y = y;
		}

        public Vector2Int Normalized() {
            if (x == 0 && y == 0) return Vector2Int.Zero;
            double length = Math.Sqrt(x * x + y * y);
            return new Vector2Int((int)Math.Round((double)x / length),
                                  (int)Math.Round((double)y / length));
		}

        public int Manhattan(Vector2Int v) {
            return  Math.Abs(x - v.x) + Math.Abs(y - v.y);
        }

        public float Magnitude() {
            return (float)Math.Sqrt(x * x + y * y);
		}

        public static bool operator ==(Vector2Int a, Vector2Int b) {
			return a.x == b.x && a.y == b.y;
		}
        public static bool operator !=(Vector2Int a, Vector2Int b) {
			return a.x != b.x || a.y != b.y;
		}

        public static Vector2Int operator *(int i, Vector2Int v) {
            return new Vector2Int(i * v.x, i * v.y);
		}

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) {
            return new Vector2Int(a.x + b.x, a.y + b.y);
		}

        public static Vector2Int operator -(Vector2Int a, Vector2Int b) {
            return new Vector2Int(a.x - b.x, a.y - b.y);
		}

        public override bool Equals(object obj) {
            return obj is Vector2Int @int &&
                   x == @int.x &&
                   y == @int.y;
        }

        public override string ToString() {
            return String.Format("({0}, {1})", x, y);
        }

        public override int GetHashCode() {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }
}
