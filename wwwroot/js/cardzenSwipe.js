// cardzenSwipe.js - Hammer.js swipe integration for Blazor TransactionList
window.cardzenSwipe = {
    scrollToTop: function() {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    },
    resetSwipe: function(txnId) {
        if (!txnId) return;
        var selector = '.swipeable-item[data-txn-id="' + txnId + '"]';
        var el = document.querySelector(selector);
        if (el && el.classList.contains('swiped')) {
            el.classList.remove('swiped');
        }
    },
    registerSwipeActions: function (itemSelector, actionsSelector, dotNetRef) {
        if (!window.Hammer) return;
        window.cardzenSwipe._attachSwipe(itemSelector, actionsSelector, dotNetRef);
        if (!window.cardzenSwipe._observer) {
            var container = document.querySelector(itemSelector)?.parentElement?.parentElement || document.body;
            window.cardzenSwipe._observer = new MutationObserver(function (mutations) {
                mutations.forEach(function (mutation) {
                    if (mutation.addedNodes && mutation.addedNodes.length > 0) {
                        window.cardzenSwipe._attachSwipe(itemSelector, actionsSelector, dotNetRef);
                    }
                });
            });
            window.cardzenSwipe._observer.observe(container, { childList: true, subtree: true });
        }
    },
    _attachSwipe: function (itemSelector, actionsSelector, dotNetRef) {
        document.querySelectorAll(itemSelector).forEach(function (el) {
            if (el._hammerAttached) return;
            console.log('[cardzenSwipe] Attach Hammer.js for', el);
            var txnId = el.getAttribute('data-txn-id');
            // Hammer.js for touch
            var mc = new Hammer(el);
            mc.on('swipeleft swiperight', function (ev) {
                let direction = ev.type === 'swipeleft' ? 'left' : 'right';
                if (window.innerWidth <= 640) {
                    if (ev.type === 'swipeleft') {
                        el.classList.add('swiped');
                    } else if (ev.type === 'swiperight') {
                        el.classList.remove('swiped');
                    }
                }
                if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                    dotNetRef.invokeMethodAsync('OnSwipe', direction, txnId);
                }
            });
            // Mouse drag for desktop
            let startX = null;
            let dragging = false;
            el.addEventListener('mousedown', function (e) {
                if (e.button !== 0) return; // only left mouse
                startX = e.clientX;
                dragging = true;
            });
            el.addEventListener('mousemove', function (e) {
                if (!dragging || startX === null) return;
                let dx = e.clientX - startX;
                // threshold: 60px
                if (dx < -60) {
                    if (window.innerWidth <= 640) {
                        el.classList.add('swiped');
                    }
                    dragging = false;
                    if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                        dotNetRef.invokeMethodAsync('OnSwipe', 'left', txnId);
                    }
                } else if (dx > 60) {
                    if (window.innerWidth <= 640) {
                        el.classList.remove('swiped');
                    }
                    dragging = false;
                    if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                        dotNetRef.invokeMethodAsync('OnSwipe', 'right', txnId);
                    }
                }
            });
            el.addEventListener('mouseup', function (e) {
                startX = null;
                dragging = false;
            });
            el.addEventListener('mouseleave', function (e) {
                startX = null;
                dragging = false;
            });
            el._hammerAttached = true;
        });
    }
};
