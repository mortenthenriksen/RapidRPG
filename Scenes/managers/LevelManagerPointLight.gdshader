shader_type canvas_item;

// Uniforms for customization
uniform vec3 glow_color = vec3(1.0, 1.0, 0.8); // Light gold glow
uniform float glow_intensity = 1.2; // Glow brightness
uniform float pulse_speed = 1.0; // Pulsation speed
uniform float alpha = 0.8; // Transparency
uniform sampler2D glow_texture; // Glow texture (optional)

// Fallback for missing texture
void fragment() {
    // Sample the glow texture if provided, else fallback to uniform color
    vec4 tex_color = texture(glow_texture, UV);
    if (tex_color.a == 0.0) tex_color = vec4(1.0); // Default to white if no texture

    // Distance-based fade for smooth edges
    float dist = length(SCREEN_UV - vec2(0.5)); // Distance from center
    float fade = 1.0 - smoothstep(0.0, 0.8, dist); // Fade outwards

    // Pulsating effect using sine wave
    float pulse = 0.5 + 0.5 * sin(TIME * pulse_speed); // Pulsating glow

    // Combine texture color and pulsating glow
    vec3 glow = glow_color * glow_intensity * pulse * fade; // Dynamic glow
    vec3 result = mix(tex_color.rgb, glow, 0.8); // Blend texture with glow

    // Final color with transparency
    COLOR = vec4(result, tex_color.a * alpha * fade);
}
