shader_type canvas_item;
uniform float deformationAmount;
uniform float xScale;
uniform float yScale;

void vertex() {
	VERTEX.x += xScale * (2.0 * UV.x - 1.0) * 64.0 * deformationAmount;
	VERTEX.y += yScale * deformationAmount * 64.0 * (1.0 - UV.y);
}