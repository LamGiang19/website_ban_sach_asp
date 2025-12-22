let isInitialized = false;

function updateCartCount() {
    $.ajax({
        url: '/GioHang/GetCartCount',
        type: 'GET',
        success: function(response) {
            if (response && response.count !== undefined) {
                $('.cart-count').text(response.count);
            }
        },
        error: function() {
            // User not logged in, show 0
            $('.cart-count').text('0');
        }
    });
}

function updateCartModal() {
    $.ajax({
        url: '/GioHang/GetCartItems',
        type: 'GET',
        success: function(response) {
            const cartItems = $('#cartItems');
            const cartTotal = $('#cartTotal');
            
            if (!cartItems.length || !cartTotal.length) return;
            
            if (!response.items || response.items.length === 0) {
                cartItems.html('<p class="text-center text-muted">Giỏ hàng của bạn đang trống</p>');
                cartTotal.text('0₫');
                return;
            }
            
            let html = '<table class="table table-sm">';
            let total = response.tongTien || 0;
            
            response.items.forEach(item => {
                html += `
                    <tr>
                        <td>
                            <img src="${item.hinhAnh}" alt="${item.tenSach}" style="width: 50px; height: 50px; object-fit: cover;" onerror="this.src='/images/sach/default.jpg'">
                        </td>
                        <td>
                            <strong>${item.tenSach}</strong><br>
                            <small class="text-muted">${item.gia.toLocaleString('vi-VN')}₫</small>
                        </td>
                        <td>
                            <small>SL: ${item.soLuong}</small>
                        </td>
                        <td>
                            <strong>${item.thanhTien.toLocaleString('vi-VN')}₫</strong>
                        </td>
                    </tr>
                `;
            });
            
            html += '</table>';
            cartItems.html(html);
            cartTotal.text(total.toLocaleString('vi-VN') + '₫');
        },
        error: function() {
            const cartItems = $('#cartItems');
            const cartTotal = $('#cartTotal');
            if (cartItems.length) {
                cartItems.html('<p class="text-center text-muted">Vui lòng đăng nhập để xem giỏ hàng</p>');
            }
            if (cartTotal.length) {
                cartTotal.text('0₫');
            }
        }
    });
}

// Toast Notification Functions
function showToast(message, type = 'success') {
    const container = $('#toastContainer');
    if (!container.length) {
        $('body').append('<div id="toastContainer" class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 9999;"></div>');
    }
    
    const template = document.getElementById('toastTemplate');
    if (!template) return;
    
    const toastElement = template.content.cloneNode(true);
    const toast = $(toastElement.querySelector('.toast'));
    
    // Set type and icon - Icon mềm mại hơn
    const icons = {
        success: 'far fa-check-circle',
        error: 'far fa-times-circle',
        warning: 'far fa-exclamation-triangle',
        info: 'far fa-info-circle'
    };
    
    toast.addClass(type);
    toast.find('.toast-icon').addClass(icons[type] || icons.success);
    toast.find('.toast-message').text(message);
    
    $('#toastContainer').append(toast);
    const bsToast = new bootstrap.Toast(toast[0], {
        autohide: true,
        delay: type === 'error' ? 5000 : 3000
    });
    bsToast.show();
    
    toast.on('hidden.bs.toast', function() {
        $(this).remove();
    });
}

function showSuccess(message) {
    showToast(message, 'success');
}

function showError(message) {
    showToast(message, 'error');
}

function showWarning(message) {
    showToast(message, 'warning');
}

function showInfo(message) {
    showToast(message, 'info');
}

// Confirmation Modal Function
function showConfirm(message, onConfirm, onCancel = null) {
    const modal = $('#confirmModal');
    const confirmBtn = $('#confirmBtn');
    const confirmMessage = $('#confirmMessage');
    
    confirmMessage.text(message);
    
    // Remove previous handlers
    confirmBtn.off('click');
    
    // Set new handler
    confirmBtn.on('click', function() {
        if (onConfirm) {
            onConfirm();
        }
        bootstrap.Modal.getInstance(modal[0]).hide();
    });
    
    // Handle cancel
    modal.off('hidden.bs.modal');
    modal.on('hidden.bs.modal', function() {
        if (onCancel) {
            onCancel();
        }
    });
    
    const bsModal = new bootstrap.Modal(modal[0]);
    bsModal.show();
}

// Legacy function for backward compatibility
function showNotification(message) {
    showSuccess(message);
}

$(document).ready(function() {
    if (isInitialized) return;
    isInitialized = true;
    
    updateCartCount();
    
    $(document).on('click', '.add-to-cart', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        const $btn = $(this);
        const sachId = $btn.data('id');
        const soLuong = 1;
        
        if (!sachId) return;
        
        $btn.prop('disabled', true);
        
        $.ajax({
            url: '/GioHang/AddToCart',
            type: 'POST',
            data: { sachId: sachId, soLuong: soLuong },
            success: function(response) {
                if (response.success) {
                    showSuccess(response.message || 'Đã thêm vào giỏ hàng');
                    updateCartCount();
                    updateCartModal();
                } else {
                    showError(response.message || 'Có lỗi xảy ra');
                }
            },
            error: function(xhr) {
                if (xhr.status === 401 || xhr.responseJSON?.message?.includes('đăng nhập')) {
                    showConfirm(
                        'Vui lòng đăng nhập để thêm sách vào giỏ hàng. Bạn có muốn chuyển đến trang đăng nhập?',
                        function() {
                            window.location.href = '/TaiKhoan/DangNhap';
                        }
                    );
                } else {
                    showError('Có lỗi xảy ra. Vui lòng thử lại.');
                }
            },
            complete: function() {
                $btn.prop('disabled', false);
            }
        });
    });
    
    $('#cartModal').on('show.bs.modal', function() {
        updateCartModal();
    });
    
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
