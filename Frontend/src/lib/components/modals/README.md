# Modal Component System

## Base Modal Component

The `Modal.svelte` component is the foundation for all modals in the application. It provides:

- Native HTML `<dialog>` element with proper accessibility
- Automatic centering with pure Tailwind CSS
- Backdrop click to close
- ESC key support
- Focus trap (native)
- Dark mode support
- Multiple sizes (sm, md, lg, full)
- Smooth animations

## Creating New Modal Types

To create a new modal type, create a new component that wraps the base Modal:

### Pattern

```svelte
<script lang="ts">
  import Modal from './Modal.svelte';

  interface Props {
    isOpen?: boolean;
    // Your custom props here
    onClose?: () => void;
  }

  let {
    isOpen = $bindable(false),
    // destructure custom props
    onClose
  }: Props = $props();

  function handleAction() {
    // Your custom logic
    isOpen = false;
  }
</script>

<Modal bind:isOpen size="md" onClose={onClose}>
  {#snippet header()}
    <!-- Your header content -->
  {/snippet}

  {#snippet content()}
    <!-- Your content -->
  {/snippet}

  {#snippet footer()}
    <!-- Your footer with action buttons -->
  {/snippet}
</Modal>
```

### Examples

#### ConfirmModal
Confirmation dialogs with confirm/cancel actions.

```svelte
<ConfirmModal
  bind:isOpen={showConfirm}
  title="Confirm Action"
  message="Are you sure you want to proceed?"
  confirmText="Yes, continue"
  cancelText="Cancel"
  onConfirm={() => console.log('Confirmed!')}
/>
```

#### AlertModal
Simple alert messages with a single OK button.

```svelte
<AlertModal
  bind:isOpen={showAlert}
  title="Success"
  message="Your changes have been saved successfully."
  buttonText="OK"
  onClose={() => console.log('Alert closed')}
/>
```

#### FormModal
Forms with submit/cancel actions and custom form content.

```svelte
<FormModal
  bind:isOpen={showForm}
  title="Add New Item"
  submitText="Save"
  cancelText="Cancel"
  onSubmit={handleFormSubmit}
  onCancel={() => console.log('Form cancelled')}
>
  {#snippet formContent()}
    <form class="space-y-4">
      <div>
        <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
          Name
        </label>
        <input
          id="name"
          type="text"
          bind:value={formData.name}
          class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500"
          required
        />
      </div>
    </form>
  {/snippet}
</FormModal>
```

## Base Modal Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `isOpen` | `boolean` | `false` | Controls modal visibility (bindable) |
| `size` | `'sm' \| 'md' \| 'lg' \| 'full'` | `'md'` | Modal size |
| `showClose` | `boolean` | `true` | Show/hide the X close button |
| `closeOnBackdrop` | `boolean` | `true` | Allow closing by clicking backdrop |
| `onClose` | `() => void` | `undefined` | Callback when modal closes |
| `header` | `Snippet` | `undefined` | Header content snippet |
| `content` | `Snippet` | required | Main content snippet |
| `footer` | `Snippet` | `undefined` | Footer content snippet |

## Modal Sizes

- **sm**: Max width 24rem (384px)
- **md**: Max width 42rem (672px)
- **lg**: Max width 56rem (896px)
- **full**: 95vw x 95vh (nearly fullscreen)

## Usage Tips

1. **Always use `bind:isOpen`** to allow the modal to close itself when the user clicks the backdrop or presses ESC
2. **Use snippets for flexibility** - The base Modal component uses Svelte 5 snippets for maximum customization
3. **Create specialized wrappers** - Instead of using the base Modal directly everywhere, create purpose-specific wrappers like ConfirmModal or AlertModal
4. **Handle form submissions properly** - When using forms inside modals, prevent default form submission and handle it with your own logic
5. **Clean up state on close** - Remember to reset any form data or temporary state when the modal closes

## Accessibility Features

- Native `<dialog>` element for built-in accessibility
- Proper ARIA attributes (`aria-modal="true"`)
- Focus trap (native to dialog element)
- ESC key to close
- Screen reader friendly
- Keyboard navigation support

## Styling

All styling is done with Tailwind CSS utility classes. No custom CSS is used. This makes it easy to:
- Customize colors and spacing
- Maintain consistent design system
- Support dark mode seamlessly
- Override styles when creating custom modal types
