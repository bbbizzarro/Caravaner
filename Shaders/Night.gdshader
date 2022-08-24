shader_type canvas_item;
uniform float transition_amount;
uniform sampler2D palette;

void fragment() {
	float t = clamp(transition_amount, 0, 1);
	vec4 bg = texture(SCREEN_TEXTURE, SCREEN_UV);
	float avg = (bg.r + bg.g + bg.b) /3f;
	float cmax = max(bg.r, max(bg.g, bg.b));
	float cmin = min(bg.r, min(bg.g, bg.b));
	//vec4 c = texture(palette, vec2(cmax, 0.5));
	vec4 c = texture(palette, vec2(avg, 0.5));
	COLOR = c * t + bg * (1f - t);
}