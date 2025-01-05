shader_type canvas_item;

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    vec3 glow_color = vec3(1.0, 1.0, 0.8); // Light gold glow

    // Boost brightness for a glowing effect
    vec3 result = mix(tex_color.rgb, glow_color, 0.5) * 1.2;
	
	float alpha = tex_color.a * 0.78; // transparency = 50%

    COLOR = vec4(result, alpha);
}