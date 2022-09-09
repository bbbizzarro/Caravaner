shader_type canvas_item;
uniform vec4 shadow_color;
uniform sampler2D sprite;

void fragment() {
	COLOR = texture(sprite, UV);
	COLOR = vec4(shadow_color.r, shadow_color.g, shadow_color.b, COLOR.a);
}

void vertex() {
	VERTEX = vec2(VERTEX.x, VERTEX.y * 0.5f);
}







