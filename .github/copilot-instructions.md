# CardZen Project Instructions

You are a .NET and Frontend expert, supporting the development of **CardZen** - a Blazor WebAssembly PWA application using Tailwind CSS and DaisyUI.

## 1. Core Technologies
- **Framework:** Blazor WebAssembly (.NET 8/9+)
- **UI Library:** DaisyUI (Tailwind CSS plugin)
- **Architecture:** PWA (Progressive Web App) - Prioritize fast loading and lightweight performance.
- **Primary Documentation:** https://daisyui.com/llms.txt

## 2. DaisyUI UI Code Principles
- **No-JS Interop:** Always prioritize using "Checkbox Hack" or pure CSS from DaisyUI for interactive components like Modal, Drawer, Dropdown, Swap. Minimize JSRuntime calls.
- **Class Structure:** Use DaisyUI classes instead of writing overly long Tailwind utilities (e.g., use `btn btn-primary` instead of `px-4 py-2 bg-blue-500...`).
- **Responsive:** Mobile-first design. Use Tailwind modifiers (`sm:`, `md:`, `lg:`) to optimize display for both mobile and desktop.
- **Dark Mode:** Default support for Dark mode through DaisyUI's theme mechanism (`data-theme`).

## 3. Blazor & C# Rules
- **Componentization:** Break down the UI into Shared Components in Blazor for reusability.
- **Parameters:** Use `[Parameter]` to pass CSS classes into components (e.g., `[Parameter] public string Class { get; set; }`) for flexible UI customization from outside.
- **State Management:** Use `InputCheckbox` or `bool` variables in C# to control the `checked` state of DaisyUI components (such as Modal toggle).

## 4. Specific Code Generation Guidelines
- When creating Modals: Always use `<input type="checkbox" id="my_modal" class="modal-toggle" />`.
- When creating Layout: Use `drawer`, `drawer-content`, and `drawer-side` structure for navigation menu.
- Always check the latest updates from DaisyUI's `llms.txt` file to use correct v5 class names.

## 5. CardZen Project Goals
- UI must be clean and refined (Zen style).
- Extremely fast response on mobile devices.
- Easy to maintain code, minimize dependency on third-party JS libraries.
