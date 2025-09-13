#include <windows.h>
#include <stdio.h>

HHOOK hHook;

BOOL IsAllowedKey(DWORD vkCode) {
    switch (vkCode) {
        case VK_LEFT:
        case VK_RIGHT:
        case VK_UP:
        case VK_DOWN:
        case VK_SPACE:
        case VK_RETURN:
        case VK_ESCAPE: // ESC agora permitido
        case 0x57: // W
        case 0x53: // S
        case 0x46: // F
        case 0x49: // I
            return TRUE;
        default:
            return FALSE;
    }
}

LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
    if (nCode == HC_ACTION) {
        KBDLLHOOKSTRUCT *p = (KBDLLHOOKSTRUCT*)lParam;

        if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) {
            if (!IsAllowedKey(p->vkCode)) {
                return 1; // bloqueia tecla
            }
        }
    }
    return CallNextHookEx(hHook, nCode, wParam, lParam);
}

int main() {
    RECT rect;
    rect.left   = GetSystemMetrics(SM_CXSCREEN) - 1; 
    rect.top    = 0;                                 
    rect.right  = GetSystemMetrics(SM_CXSCREEN);     
    rect.bottom = 1;                                 
    ClipCursor(&rect);

    // --- hook do teclado ---
    hHook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, NULL, 0);
    if (!hHook)
        return 1;

    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    UnhookWindowsHookEx(hHook);
    ClipCursor(NULL);
    return 0;
}
