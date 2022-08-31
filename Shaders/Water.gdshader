shader_type canvas_item;
uniform sampler2D noise_pattern;
uniform float speed;
uniform float scale_factor;
uniform vec4 water_color;
uniform vec4 highlight_color;

void fragment(){
	float scale = 1.0 / scale_factor;
	float scale2 = 0.8* 1.0 / scale_factor;
	//vec2 uv = vec2(UV.x, mod(UV.y - mod(TIME, 1.0 / speed), 1.0));
	//vec2 uv = vec2(UV.x, mod(UV.y - speed* TIME, 1.0));
	vec2 uv = vec2(UV.x, mod(UV.y - 0.01*((sin(TIME) + 1.0)/ 2.0), 1.0));
	vec2 uv2 = vec2(UV.x, mod(UV.y - 0.001*TIME, 1.0));
	vec2 noise_uv = vec2(mod(uv.x, scale) / scale, mod(uv.y * 4.0, scale) / scale);
	vec2 noise_uv2 = vec2(mod(uv2.x, scale2) / scale, mod(uv2.y * 4.0, scale2) / scale2);
	vec4 noise = 0.5 * texture(noise_pattern, noise_uv) + 0.5* texture(noise_pattern, noise_uv2);
	//float c = (sin(speed * TIME) + 1.0)/ 2.0;
	float c = step(noise.r, 0.8);
	//COLOR = noise * vec4(UV.x,1.0- c, UV.y, 1.0 );
	COLOR = c * water_color + (1.0 -c) * highlight_color;
	COLOR.a = 1.0;
}