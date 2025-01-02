import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from './AuthContext';
import './css/OrderSummary.css';

function OrderSummary() {
    const location = useLocation();
    const navigate = useNavigate();
    const { selectedSeats = [], movieId, showtime } = location.state || {};

    const { userId } = useAuth();

    const [items, setItems] = useState([]);
    const [selectedItems, setSelectedItems] = useState([]);
    const [timeRemaining, setTimeRemaining] = useState(300); // 5 phút
    const [email, setEmail] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const seatPrice = 100000; // Giá vé cố định

    // Lấy danh sách đồ ăn/uống từ API và khởi tạo giá trị mặc định cho selectedItems
    useEffect(() => {
        if (!Array.isArray(selectedSeats) || selectedSeats.length === 0 || !showtime || !movieId) {
            alert('Thông tin không đầy đủ, vui lòng thử lại!');
            navigate('/');
            return;
        }
        fetchItems();
    }, [selectedSeats, showtime, movieId, navigate]);

    const fetchItems = async () => {
        try {
            const response = await axios.get('http://localhost:5175/api/Item');
            setItems(response.data);

            // Đặt giá trị mặc định cho selectedItems
            const defaultSelectedItems = response.data.map((item) => ({
                itemId: item.itemId,
                quantity: 0,
            }));
            setSelectedItems(defaultSelectedItems);
        } catch (error) {
            console.error('Error fetching items:', error);
        }
    };

    // Đếm ngược thời gian
    useEffect(() => {
        const interval = setInterval(() => {
            setTimeRemaining((prev) => {
                if (prev <= 0) {
                    clearInterval(interval);
                    alert('Phiên đặt vé đã hết! Vui lòng thử lại.');
                    navigate('/');
                    return 0;
                }
                return prev - 1;
            });
        }, 1000);
        return () => clearInterval(interval);
    }, [navigate]);

    const formatTime = () => {
        const minutes = Math.floor(timeRemaining / 60);
        const seconds = timeRemaining % 60;
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    const handleQuantityChange = (itemId, quantity) => {
        setSelectedItems((prev) =>
            prev.map((item) =>
                item.itemId === itemId
                    ? { ...item, quantity: quantity > 0 ? quantity : 0 }
                    : item
            )
        );
    };

    const calculateTotal = () => {
        const ticketTotal = selectedSeats.length * seatPrice;
        const itemsTotal = selectedItems.reduce((acc, item) => {
            const product = items.find((i) => i.itemId === item.itemId);
            return acc + (product ? product.price * item.quantity : 0);
        }, 0);
        return ticketTotal + itemsTotal;
    };

    const handlePlaceOrder = async () => {
        if (!email) {
            alert('Vui lòng nhập email!');
            return;
        }

        if (!userId) {
            alert('Lỗi: Không xác định được UserId. Vui lòng đăng nhập lại!');
            navigate('/login');
            return;
        }

        // Chuẩn bị payload
        const ticketData = selectedSeats.map((seat) => ({
            showtimeId: showtime.showtimeId,
            seatId: seat.seatId,
            price: seatPrice,
            userId, // Thêm userId vào từng vé
            movieId,
            theaterId: showtime.theaterId, // Lấy theaterId từ showtime
        }));

        const itemOrders = selectedItems.filter((item) => item.quantity > 0).map((item) => ({
            itemId: item.itemId,
            quantity: item.quantity,
        }));

        const orderData = {
            userId,
            tickets: ticketData,
            itemOrders,
        };

        try {
            setIsLoading(true);
            const response = await axios.post('http://localhost:5175/api/Orders/CreateOrderAndTickets', orderData);

            alert(`Đặt vé thành công! Mã đơn hàng: ${response.data.id}`);
            navigate('/');
        } catch (error) {
            if (error.response) {
                console.error('Error response:', error.response.data);
                alert(`Lỗi từ API: ${JSON.stringify(error.response.data)}`);
            } else {
                console.error('Error placing order:', error.message);
                alert('Đã xảy ra lỗi khi đặt vé. Vui lòng thử lại!');
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="order-summary">
            <h1>Thông Tin Đặt Vé</h1>
            <h2>Suất Chiếu</h2>
            <p>{`${new Date(showtime?.showDate).toLocaleDateString()} - ${showtime?.showHour}`}</p>

            <h2>Ghế Đã Chọn</h2>
            <div className="seats-list">
                {selectedSeats.map((seat) => (
                    <p key={seat.seatId}>
                        Ghế: {seat.row}{seat.number}
                    </p>
                ))}
            </div>

            <h2>Đồ Ăn Kèm</h2>
            <div className="item-grid-container">
                <div className="item-grid">
                    {items.map((item) => (
                        <div key={item.itemId} className="item-card">
                            <p>{item.name} - {item.price} VND</p>
                            <input
                                type="number"
                                min="0"
                                value={
                                    selectedItems.find((selectedItem) => selectedItem.itemId === item.itemId)?.quantity || 0
                                }
                                onChange={(e) =>
                                    handleQuantityChange(item.itemId, parseInt(e.target.value) || 0)
                                }
                            />
                        </div>
                    ))}
                </div>
            </div>

            <h3>Tổng Tiền: {calculateTotal()} VND</h3>
            <h3>Thời Gian Còn Lại: {formatTime()}</h3>

            <h2>Thông Tin Liên Hệ</h2>
            <input
                type="email"
                placeholder="Nhập email để nhận thông tin vé"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
            />

            <button onClick={handlePlaceOrder} disabled={!email || isLoading}>
                {isLoading ? 'Đang xử lý...' : 'Xác Nhận'}
            </button>
        </div>
    );
}

export default OrderSummary;
