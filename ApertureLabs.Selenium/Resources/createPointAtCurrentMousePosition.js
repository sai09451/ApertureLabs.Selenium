function handler(evt) {
    document.removeEventListener('mousemove', handler);
    var el = document.createElement('div');
    el.style.width = '50px';
    el.style.height = '50px';
    el.style.border = '5px solid red';
    el.style.borderRadius = '50%';
    el.style.position = 'absolute';
    el.style.left = evt.pageX - 25 + 'px';
    el.style.top = evt.pageY - 25 + 'px';
    document
        .getElementsByTagName('body')[0]
        .appendChild(el);
    {resolve}(JSON.stringify({ X: evt.pageX, Y: evt.pageY }));
}
document.addEventListener('mousemove', handler);