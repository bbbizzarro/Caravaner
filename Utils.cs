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

		public Vector2Int(int x, int y) {
			this.x = x;
			this.y = y;
		}

        public static bool operator ==(Vector2Int a, Vector2Int b) {
			return a.x == b.x && a.y == b.y;
		}
        public static bool operator !=(Vector2Int a, Vector2Int b) {
			return a.x != b.x || a.y != b.y;
		}

        public override bool Equals(object obj) {
            return obj is Vector2Int @int &&
                   x == @int.x &&
                   y == @int.y;
        }

        public override string ToString() {
            return String.Format("('{0}', '{1}'", x, y);
        }
    }
}
