const header = document.querySelector('.header');
const back = document.querySelector('.backToTop');
back.addEventListener('click', function (e) {
    window.scrollTo({ top: 0 });
})
window.addEventListener('scroll', function (e) {
    
        const scrollY = this.window.pageYOffset;
    const headerH = header.offsetHeight;
    if (scrollY > headerH) {
        back.classList.add('sup');
    }
    else {
        back.classList.remove('sup');

    }
    
})

const menu = document.querySelector('.menu-mb');
const ham = document.querySelector('.ham');
ham.addEventListener('click', function (e) {
    e.target.classList.toggle('active-ham');
    menu.classList.toggle('active');

})



//const user = document.querySelector('.user');
//user.addEventListener("click", function (e) {
//    const form = ` <form class="form-dn">
//        <div class="form-content">
            
//            <a href="" class="btn-dn">Đăng nhập</button>
//            <a href="" class="btn-dn">Đăng ky</button>
//        </div>
//    </form>`;
//    document.body.insertAdjacentHTML("beforeend", form);
//})
document.body.addEventListener("click", function (e) {
    if (e.target.matches(".form-dn")) {
        e.target.parentNode.removeChild(e.target);
    }
    if (e.target.matches(".dn-cl")) {
        const formDN = document.querySelector(".form-dn");
        formDN.parentNode.removeChild(formDN);
    }
    if (e.target.matches('.lightbox')) {
        e.target.parentNode.removeChild(e.target);
    }


});



//Tooltip
const texts = document.querySelectorAll(".product a.namePro");
if (window.innerWidth > 760) {
    [...texts].forEach(item => item.addEventListener("mouseenter", function (e) {

        const title = e.target.textContent;
        const tooltipDiv = document.createElement("div");
        tooltipDiv.className = "tooltip-text";
        tooltipDiv.textContent = title;
        document.body.appendChild(tooltipDiv);
        const cords = event.target.getBoundingClientRect();
        const { top, left, width } = cords;
        const tooltipHeight = tooltipDiv.offsetHeight;
        const triangleHeight = 20;
        tooltipDiv.style.left = `${left - width / 4}px`;
        tooltipDiv.style.top = `${top - tooltipHeight - triangleHeight}px`;
    }));
    [...texts].forEach(item => item.addEventListener("mouseleave", function (e) {
        const tooltip = document.querySelector(".tooltip-text");
        if (!tooltip) return;
        tooltip.parentNode.removeChild(tooltip);
    }));
} else {
    window.addEventListener('scroll', function (e) {
        const menu = document.querySelector('.menu-mb');
        const ham = document.querySelector('.ham');
        if (menu.classList.contains("active")) {
            menu.classList.remove('active');
            ham.classList.remove('active-ham');
        }

    })
}



