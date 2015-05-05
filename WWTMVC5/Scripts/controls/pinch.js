



wwt.pincher = (function () {

    var api = { init: init };

    var onZoom, // this is a function pointer that calls wwtControlInstance.zoom(ratio);
        el,//in this case it is the pinch overlay div, but it would be canvas for you.
        curCoords,//touch events only
        newCoords,//touch events only
        pointers = [];//pointer events only - tracks 2 objects with id, x, and y values

    function init(e, callback) {
        el = e;
        console.log(e);
        if (typeof e[0].style.msTouchAction != 'undefined') {
            e[0].style.msTouchAction = 'none';
        }
        // attach to all of these events. Browser will ignore events it does not expose
        // note that this is jQuery syntax - you would use element.attachEventLister 
        // or something similar in scriptsharp
        el.on('pointerdown MSPointerDown', pointerDown);
        el.on('pointerup MSPointerUp', pointerUp);
        el.on('touchstart', touchStart);
        
        onZoom = callback;
    }
   
    
    var pointerDown = function (event) {
        var e = event.originalEvent;
        pointers.push({//add to the array of pointers currently down on the screen.
            id: e.pointerId,
            x: e.pageX,
            y: e.pageY
        });
        if (pointers.length === 2) {// we only care if we have exactly 2 pointers
            el.on('pointermove MSPointerMove', pointerMove);
            cancelEvent(event);
        }
    };
    var pointerMove = function (event) {
        
        var e = event.originalEvent;
        var curPtr; // this is the pointer obj that changed
        var ptrIndex;
        var stalePtr; // this is the pointer obj that didn't change
        
        $.each(pointers, function(i,ptr) {
            if (i < 2 && ptr.id === e.pointerId) {
                curPtr = ptr;
                ptrIndex = i;
            }
        });
        // do this check in the case of 3 fingers. We don't support 3 finger zoom yet
        if (ptrIndex) {
            //determine which is the stale ptr
            stalePtr = pointers[(ptrIndex === 0) ? 1 : 0];
            
            //find our old distance between the points
            var oldDist = getDistance(pointers[0], pointers[1]);

            //set the current ptr to have this events x/y data
            curPtr.x = e.pageX;
            curPtr.y = e.pageY;
            
            //get the new distance 
            var newDist = getDistance(curPtr, stalePtr);

            // get the ratio and trigger the callback
            var ratio = oldDist / newDist;
            
            onZoom(ratio);
        }
        
        cancelEvent(event);
    };
    var pointerUp = function(event) {
        var e = event.originalEvent;
        var ptrIndex;
        // find the pointer that left the screen by its id
        $.each(pointers, function(i,ptr) {
            if (ptr.id === e.pointerId) {
                ptrIndex = i;
            }
        });
        // and remove the pointer from the pointers array
        pointers.splice(ptrIndex);//splice just removes an item from the array and shortens the array

        // untrap events when not two fingers
        if (pointers.length !== 2) {
            el.off('pointermove MSPointerMove', pointerMove);//detach events
        }
    };

    var touchStart = function (event) {
        console.log('ts');
        var touches = event.originalEvent.targetTouches;
        if (touches && touches.length === 2) { //two fingers down
            cancelEvent(event);
            curCoords = [{
                    x: touches[0].pageX,
                    y: touches[0].pageY
                }, {
                    x: touches[1].pageX,
                    y: touches[1].pageY
                }
            ];
            el.on('touchmove', touchMove);
            el.on('touchend', touchEnd);
        }
    };

    var touchMove = function (event) {
        var touches = event.originalEvent.targetTouches;
        if (touches.length == 2) {
            newCoords = [
                {
                    x: touches[0].pageX,
                    y: touches[0].pageY
                }, {
                    x: touches[1].pageX,
                    y: touches[1].pageY
                }
            ];
            var oldDist = getDistance(curCoords[0], curCoords[1]);
            var newDist = getDistance(newCoords[0], newCoords[1]);
            var ratio = oldDist / newDist;
            onZoom(ratio);
            curCoords = newCoords;
            cancelEvent(event);
            return false;
        }
        
    };

    var touchEnd = function (event) {
        el.off('touchmove mousemove', touchMove);
        el.off('touchend', touchEnd);
    };

    var getDistance = function (a, b) {
        var x, y;
        x = a.x - b.x;
        y = a.y - b.y;
        return Math.sqrt(x * x + y * y);
    };
    var cancelEvent = function (event) {
        event.stopPropagation();
        event.preventDefault();
    };
    return api;
})()

