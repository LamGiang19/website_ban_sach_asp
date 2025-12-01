// Giỏ hàng
let cart = JSON.parse(localStorage.getItem('cart')) || [];

// Cập nhật số lượng giỏ hàng
function updateCartCount() {
    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    $('.cart-count').text(totalItems);
}

// Thêm sản phẩm vào giỏ hàng
function addToCart(sachId, tenSach, gia, hinhAnh) {
    const existingItem = cart.find(item => item.id === sachId);
    
    if (existingItem) {
        existingItem.quantity += 1;
    } else {
        cart.push({
            id: sachId,
            name: tenSach,
            price: gia,
            image: hinhAnh || '/images/sach/default.jpg',
            quantity: 1
        });
    }
    
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartCount();
    updateCartModal();
    showNotification('Đã thêm sản phẩm vào giỏ hàng!');
}

// Xóa sản phẩm khỏi giỏ hàng
function removeFromCart(sachId) {
    cart = cart.filter(item => item.id !== sachId);
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartCount();
    updateCartModal();
}

// Cập nhật số lượng sản phẩm
function updateCartQuantity(sachId, quantity) {
    const item = cart.find(item => item.id === sachId);
    if (item) {
        item.quantity = Math.max(1, parseInt(quantity));
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartModal();
    }
}

// Cập nhật modal giỏ hàng
function updateCartModal() {
    const cartItems = $('#cartItems');
    const cartTotal = $('#cartTotal');
    
    if (cart.length === 0) {
        cartItems.html('<p class="text-center text-muted">Giỏ hàng của bạn đang trống</p>');
        cartTotal.text('0₫');
        return;
    }
    
    let html = '<table class="table table-sm">';
    let total = 0;
    
    cart.forEach(item => {
        const itemTotal = item.price * item.quantity;
        total += itemTotal;
        
        html += `
            <tr>
                <td>
                    <img src="${item.image}" alt="${item.name}" style="width: 50px; height: 50px; object-fit: cover;">
                </td>
                <td>
                    <strong>${item.name}</strong><br>
                    <small class="text-muted">${item.price.toLocaleString('vi-VN')}₫</small>
                </td>
                <td>
                    <input type="number" class="form-control form-control-sm" 
                           value="${item.quantity}" min="1" 
                           onchange="updateCartQuantity(${item.id}, this.value)" 
                           style="width: 60px;">
                </td>
                <td>
                    <strong>${itemTotal.toLocaleString('vi-VN')}₫</strong>
                </td>
                <td>
                    <button class="btn btn-sm btn-danger" onclick="removeFromCart(${item.id})">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    });
    
    html += '</table>';
    cartItems.html(html);
    cartTotal.text(total.toLocaleString('vi-VN') + '₫');
}

// Hiển thị thông báo
function showNotification(message) {
    // Tạo toast notification
    const toast = $(`
        <div class="toast-notification">
            <div class="toast-content">
                <i class="fas fa-check-circle"></i>
                <span>${message}</span>
            </div>
        </div>
    `);
    
    $('body').append(toast);
    toast.fadeIn();
    
    setTimeout(() => {
        toast.fadeOut(() => toast.remove());
    }, 3000);
}

// Xử lý sự kiện khi DOM ready
$(document).ready(function() {
    // Cập nhật số lượng giỏ hàng khi trang load
    updateCartCount();
    
    // Xử lý click nút thêm vào giỏ hàng
    $(document).on('click', '.add-to-cart', function() {
        const sachId = $(this).data('id');
        const tenSach = $(this).closest('.product-card').find('.product-title a').text().trim();
        const gia = parseFloat($(this).data('price'));
        const hinhAnh = $(this).closest('.product-card').find('.product-image img').attr('src');
        
        addToCart(sachId, tenSach, gia, hinhAnh);
    });
    
    // Cập nhật modal khi mở
    $('#cartModal').on('show.bs.modal', function() {
        updateCartModal();
    });
    
    // Xử lý click category
    $('.category-link').on('click', function(e) {
        e.preventDefault();
        $('.category-link').removeClass('active');
        $(this).addClass('active');
        
        // TODO: Filter sách theo danh mục
        const categoryId = $(this).data('category-id');
        if (categoryId) {
            // Gọi API hoặc reload trang với filter
            console.log('Filter by category:', categoryId);
        }
    });
});

// CSS cho toast notification
const toastStyle = `
    <style>
        .toast-notification {
            position: fixed;
            top: 20px;
            right: 20px;
            background: #28a745;
            color: white;
            padding: 15px 20px;
            border-radius: 5px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 9999;
            display: none;
        }
        .toast-content {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .toast-content i {
            font-size: 20px;
        }
    </style>
`;
$('head').append(toastStyle);
