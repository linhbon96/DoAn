import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './css/TicketInfo.css';

function TicketInfo() {
    const [ticketInfos, setTicketInfos] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const userId = localStorage.getItem('userId'); // Giả sử UserId được lưu trong localStorage

    useEffect(() => {
        if (!userId) {
            setError('Không tìm thấy UserId. Vui lòng đăng nhập lại!');
            setLoading(false);
            return;
        }

        const fetchTicketInfos = async () => {
            try {
                const response = await axios.get(`http://localhost:5175/api/TicketInfo/user/${userId}`);
                setTicketInfos(response.data);
            } catch (error) {
                console.error('Error fetching ticket info:', error);
                setError('Không thể tải thông tin vé. Vui lòng thử lại!');
            } finally {
                setLoading(false);
            }
        };

        fetchTicketInfos();
    }, [userId]);

    const calculateTotal = (ticket) => {
        const seatPrice = ticket.ticketPrice || 0; // Giá tiền vé
        const itemTotal = ticket.itemOrders?.reduce((acc, item) => acc + (item.quantity * item.price || 0), 0) || 0;
        return seatPrice + itemTotal;
    };

    if (loading) {
        return <p>Đang tải thông tin vé...</p>;
    }

    if (error) {
        return <p className="error-message">{error}</p>;
    }

    if (ticketInfos.length === 0) {
        return <p>Không có thông tin vé để hiển thị.</p>;
    }

    return (
        <div className="ticket-info">
            <h1>Thông Tin Vé</h1>
            <div className="ticket-grid">
                {ticketInfos.map((ticket) => (
                    <div key={ticket.ticketInfoId} className="ticket-item">
                        <p><strong>Ghế:</strong> {ticket.row}{ticket.number}</p>
                        <p><strong>Người Đặt:</strong> {ticket.userName || 'Không xác định'}</p>
                        <p><strong>Chi Tiết Vé:</strong> {ticket.ticketDetails || 'Không xác định'}</p>
                        {ticket.orderId ? (
                            <>
                                <h2>Đặt Đồ Ăn</h2>
                                {ticket.itemOrders && ticket.itemOrders.length > 0 ? (
                                    <ul>
                                        {ticket.itemOrders.map((item) => (
                                            <li key={item.itemId}>
                                                {item.itemName} - Số lượng: {item.quantity} - Giá: {item.price?.toLocaleString()} VND
                                            </li>
                                        ))}
                                    </ul>
                                ) : (
                                    <p>Không có đồ ăn kèm.</p>
                                )}
                                <p><strong>Tổng Tiền (Vé + Đồ Ăn):</strong> {calculateTotal(ticket).toLocaleString()} VND</p>
                            </>
                        ) : (
                            <p>Không có đặt đồ ăn kèm.</p>
                        )}
                    </div>
                ))}
            </div>
        </div>
    );
}

export default TicketInfo;
