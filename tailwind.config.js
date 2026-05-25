/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Views/**/*.cshtml",
        "./Pages/**/*.cshtml",
        "./Components/**/*.cshtml"
    ],
    theme: {
        extend: {
            colors: {
                "brand": "#22c55e"
            }
        },
    },
    plugins: [],
}