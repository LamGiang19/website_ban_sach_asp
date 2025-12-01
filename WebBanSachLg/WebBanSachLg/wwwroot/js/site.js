// Giỏ hàng
let cart = JSON.parse(localStorage.getItem('cart')) || [];

// Flag để tránh chạy nhiều lần
let isInitialized = false;
let lastCartCount = -1;

// Cập nhật số lượng giỏ hàng - tối ưu để tránh re-render
function updateCartCount() {
    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    // Chỉ update nếu số lượng thay đổi
    if (totalItems !== lastCartCount) {
        $('.cart-count').text(totalItems);
        lastCartCount = totalItems;
    }
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
        updateCartCount();
        updateCartModal();
    }
}

// Cập nhật modal giỏ hàng
function updateCartModal() {
    const cartItems = $('#cartItems');
    const cartTotal = $('#cartTotal');
    
    if (!cartItems.length || !cartTotal.length) return;
    
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
    // Xóa notification cũ nếu có
    $('.toast-notification').remove();
    
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
    toast.fadeIn(300);
    
    setTimeout(() => {
        toast.fadeOut(300, function() {
            $(this).remove();
        });
    }, 3000);
}

// CSS cho toast notification - chỉ append một lần
if (typeof document !== 'undefined' && !document.getElementById('toast-notification-style')) {
    const toastStyle = document.createElement('style');
    toastStyle.id = 'toast-notification-style';
    toastStyle.textContent = `
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
    `;
    document.head.appendChild(toastStyle);
}

// Xử lý sự kiện khi DOM ready - chỉ chạy một lần
$(document).ready(function() {
    // Chỉ khởi tạo một lần
    if (isInitialized) return;
    isInitialized = true;
    
    // Cập nhật số lượng giỏ hàng khi trang load
    updateCartCount();
    
    // Xử lý click nút thêm vào giỏ hàng - dùng event delegation
    $(document).on('click', '.add-to-cart', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        const $btn = $(this);
        const sachId = $btn.data('id');
        const tenSach = $btn.data('name') || $btn.closest('.product-card').find('.product-title a').text().trim();
        const gia = parseFloat($btn.data('price'));
        const hinhAnh = $btn.data('image') || $btn.closest('.product-card').find('.product-image img').attr('src');
        
        if (sachId && gia) {
            addToCart(sachId, tenSach, gia, hinhAnh);
        }
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
        
        const categoryId = $(this).data('category-id');
        if (categoryId) {
            console.log('Filter by category:', categoryId);
        }
    });
});
