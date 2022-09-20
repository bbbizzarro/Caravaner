public class MapData<T> {
	public int Width {private set; get; }
	public int Height {private set; get; }

    T[,] _map;

    public MapData(int width, int height) {
		Width = width; Height = height;
		_map = new T[width, height];
        for (int x = 0; x < Width; ++x) {
            for (int y = 0; y < Height; ++y) {
                _map[x,y] = default;
            }
        }
    }
}