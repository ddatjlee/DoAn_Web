// dropdown-fix.js
// Lightweight dropdown open/close logic (no Bootstrap)
// - Toggle on click of the dropdown toggle button
// - Keep menu open while hovering over menu; close shortly after mouse leaves
// - Close when clicking outside

(function(){
    const OPEN_DELAY = 150; // ms before opening on hover (if used)
    const CLOSE_DELAY = 700; // ms before closing after mouse leaves (increased to avoid premature hiding)

    function findMenu(toggleBtn){
        // the structure: button + ul.dropdown-menu as sibling inside .nav-item
        const navItem = toggleBtn.closest('.nav-item');
        if(!navItem) return null;
        return navItem.querySelector('.dropdown-menu');
    }

    function openMenu(menu, toggleBtn){
        if(!menu) return;
        menu.classList.add('show');
        toggleBtn.setAttribute('aria-expanded','true');
    }
    function closeMenu(menu, toggleBtn){
        if(!menu) return;
        menu.classList.remove('show');
        toggleBtn.setAttribute('aria-expanded','false');
    }

    // click toggles
    document.addEventListener('click', function(e){
        const toggle = e.target.closest('.btn.dropdown-toggle, .btn.dropdown-toggle *');
        if(toggle){
            // find real button element
            const btn = toggle.closest('.btn.dropdown-toggle');
            const menu = findMenu(btn);
            if(menu.classList.contains('show')){
                closeMenu(menu, btn);
            } else {
                // close other open menus first
                document.querySelectorAll('.dropdown-menu.show').forEach(m => m.classList.remove('show'));
                document.querySelectorAll('.btn.dropdown-toggle[aria-expanded="true"]').forEach(b => b.setAttribute('aria-expanded','false'));
                openMenu(menu, btn);
            }
            e.stopPropagation();
            return;
        }

        // click outside: close all
        if(!e.target.closest('.nav-item')){
            document.querySelectorAll('.dropdown-menu.show').forEach(m => m.classList.remove('show'));
            document.querySelectorAll('.btn.dropdown-toggle[aria-expanded="true"]').forEach(b => b.setAttribute('aria-expanded','false'));
        }
    });

    // keep open while mouse is inside menu; close with delay on leave
    document.querySelectorAll('.nav-item').forEach(function(item){
        const menu = item.querySelector('.dropdown-menu');
        const btn = item.querySelector('.btn.dropdown-toggle');
        if(!menu || !btn) return;
        let closeTimer = null;

        // keep menu open while pointer is inside the nav-item or the button
        item.addEventListener('mouseenter', function(){ clearTimeout(closeTimer); });
        item.addEventListener('mouseleave', function(){
            clearTimeout(closeTimer);
            closeTimer = setTimeout(function(){ closeMenu(menu, btn); }, CLOSE_DELAY);
        });
        // also handle enter/leave for the toggle button specifically (reduces flicker when moving from button to menu)
        btn.addEventListener('mouseenter', function(){ clearTimeout(closeTimer); });
        btn.addEventListener('mouseleave', function(){
            clearTimeout(closeTimer);
            closeTimer = setTimeout(function(){ closeMenu(menu, btn); }, CLOSE_DELAY);
        });
        menu.addEventListener('mouseenter', function(){ clearTimeout(closeTimer); });
        menu.addEventListener('mouseleave', function(){
            clearTimeout(closeTimer);
            closeTimer = setTimeout(function(){ closeMenu(menu, btn); }, CLOSE_DELAY);
        });
    });

    // close on ESC
    document.addEventListener('keydown', function(e){
        if(e.key === 'Escape'){
            document.querySelectorAll('.dropdown-menu.show').forEach(m => m.classList.remove('show'));
            document.querySelectorAll('.btn.dropdown-toggle[aria-expanded="true"]').forEach(b => b.setAttribute('aria-expanded','false'));
        }
    });
})();
