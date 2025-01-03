import React, { useState, useEffect } from 'react';
import { getShowtimesByMovieId, getTheaterById, createOrderAndTickets } from '../services/apiService';
import './css/BookingPage.css';

function BookingPage() {
    const [selectedSeats, setSelectedSeats] = useState([]);
    const [snacks, setSnacks] = useState([]);
    const [showtime, setShowtime] = useState(null);
    const [theater, setTheater] = useState(null);
    const [movie, setMovie] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // Giả sử gọi API để lấy thông tin rạp, lịch chiếu và phim
        fetchShowtimeData();
    }, []);

    const fetchShowtimeData = async () => {
        // Gọi API để lấy dữ liệu (cần URL API thực tế)
        try {
            const response = await getShowtimesByMovieId(0); // Thay 0 bằng showtimeId thực tế
            const data = response.data;
            setShowtime(data.showtime);
            setTheater(data.theater);
            setMovie(data.movie);
            setLoading(false);
        } catch (error) {
            console.error('Lỗi khi tải dữ liệu:', error);
            setLoading(false);
        }
    };

    const handleSeatSelect = (seatNumber) => {
        // Kiểm tra ghế đã chọn hay chưa
        if (selectedSeats.includes(seatNumber)) {
            setSelectedSeats(selectedSeats.filter(seat => seat !== seatNumber));
        } else {
            setSelectedSeats([...selectedSeats, seatNumber]);
        }
    };

    const handleAddSnack = (snack) => {
        setSnacks([...snacks, snack]);
    };

    const handlePayment = async () => {
        const totalPrice = calculateTotalPrice();
        const payload = {
            ticketId: 0,
            movieId: movie.movieId,
            showtimeId: showtime.showtimeId,
            theaterId: theater.theaterId,
            price: showtime.price,
            seatNumbers: selectedSeats,
            showDateTime: showtime.startTime,
            totalPrice: totalPrice,
            snacks: snacks,
        };

        try {
            const response = await createOrderAndTickets(payload);
            if (response.status === 200) {
                alert('Thanh toán thành công!');
                // Xử lý thêm nếu cần sau khi thanh toán
            }
        } catch (error) {
            console.error('Lỗi khi thanh toán:', error);
        }
    };

    const calculateTotalPrice = () => {
        // Giả sử tính toán tổng giá từ ghế và đồ ăn/uống
        const seatPrice = showtime ? showtime.price : 0;
        const snackPrice = snacks.reduce((total, snack) => total + snack.price, 0);
        return seatPrice * selectedSeats.length + snackPrice;
    };

    if (loading) return <div>Đang tải dữ liệu...</div>;

    return (
        <div className="booking-page">
            <h2>Đặt vé cho phim: {movie.title}</h2>
            <p><strong>Thể loại:</strong> {movie.genre}</p>
            <p><strong>Thời lượng:</strong> {movie.duration}</p>
            <p><strong>Rạp:</strong> {theater.name} - {theater.location}</p>
            <p><strong>Giờ chiếu:</strong> {new Date(showtime.startTime).toLocaleString()}</p>

            <h3>Chọn Ghế</h3>
            <div className="seat-selection">
                {Array.from({ length: theater.rows }).map((_, row) => (
                    <div key={row} className="seat-row">
                        {Array.from({ length: theater.columns }).map((_, col) => {
                            const seatNumber = `${row + 1}-${col + 1}`;
                            return (
                                <button
                                    key={seatNumber}
                                    className={`seat ${selectedSeats.includes(seatNumber) ? 'selected' : ''}`}
                                    onClick={() => handleSeatSelect(seatNumber)}
                                >
                                    {seatNumber}
                                </button>
                            );
                        })}
                    </div>
                ))}
            </div>

            <h3>Chọn Đồ Ăn/Uống</h3>
            <div className="snacks-selection">
                <button onClick={() => handleAddSnack({ name: 'Bắp rang', price: 50000 })}>Bắp rang - 50,000đ</button>
                <button onClick={() => handleAddSnack({ name: 'Nước ngọt', price: 30000 })}>Nước ngọt - 30,000đ</button>
                <button onClick={() => handleAddSnack({ name: 'Combo Bắp & Nước', price: 70000 })}>Combo Bắp & Nước - 70,000đ</button>
            </div>

            <h4>Danh sách ghế đã chọn: {selectedSeats.join(', ')}</h4>
            <h4>Danh sách đồ ăn/uống đã chọn:</h4>
            <ul>
                {snacks.map((snack, index) => (
                    <li key={index}>{snack.name} - {snack.price}đ</li>
                ))}
            </ul>
            <h4>Tổng tiền: {calculateTotalPrice()}đ</h4>
            <button onClick={handlePayment}>Thanh Toán</button>
        </div>
    );
}

export default BookingPage;

