shader_type canvas_item;
uniform vec2 pos = vec2(-1, 0);
uniform float mag = 10;

void fragment() {
	
}

void vertex() {
	float spread = UV.x + 0.5* (1f - UV.x);
	VERTEX += 100.0* mag * vec2(pos.x *pos.x * spread, pos.y);
}