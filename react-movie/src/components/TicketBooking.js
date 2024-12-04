import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import SeatMap from './SeatMap';
import './css/TicketBooking.css';

function TicketBooking() {
    const location = useLocation();
    const navigate = useNavigate();
    const { movieId, showtime } = location.state || {};

    const [theaterInfo, setTheaterInfo] = useState(null);
    const [seats, setSeats] = useState([]);
    const [selectedSeats, setSelectedSeats] = useState([]);
    const [isChecking, setIsChecking] = useState(false);
    const seatPrice = 100000;

    useEffect(() => {
        if (!movieId || !showtime) {
            alert('Thông tin phim hoặc suất chiếu không đầy đủ. Vui lòng quay lại!');
            navigate('/');
            return;
        }

        fetchTheaterInfo(showtime.theaterId);
        fetchSeats(showtime.showtimeId);
    }, [movieId, showtime, navigate]);

    const fetchTheaterInfo = async (theaterId) => {
        try {
            const response = await axios.get(`http://localhost:5175/api/Theater/${theaterId}`);
            setTheaterInfo(response.data);
        } catch (error) {
            console.error('Error fetching theater info:', error);
            alert('Không thể tải thông tin rạp. Vui lòng thử lại!');
        }
    };

    const fetchSeats = async (showtimeId) => {
        try {
            const response = await axios.get(`http://localhost:5175/api/Seats/${showtimeId}`);
            const updatedSeats = response.data.map((seat) => {
                return {
                    ...seat,
                    isAvailable: seat.isAvailable && !seat.isBooked,
                };
            });
            setSeats(updatedSeats);
        } catch (error) {
            console.error('Error fetching seats:', error);
            alert('Không thể tải danh sách ghế. Vui lòng thử lại!');
        }
    };


    const toggleSeatSelection = (seat) => {
        setSelectedSeats((prev) =>
            prev.some((s) => s.seatId === seat.seatId)
                ? prev.filter((s) => s.seatId !== seat.seatId)
                : [...prev, seat]
        );
    };

    const handleProceedToOrder = async () => {
        if (selectedSeats.length === 0) {
            alert('Vui lòng chọn ít nhất một ghế!');
            return;
        }

        setIsChecking(true);

        try {
            // Gửi danh sách ghế đã chọn lên backend để kiểm tra trạng thái
            const response = await axios.post('http://localhost:5175/api/Seats/CheckSeats', {
                showtimeId: showtime.showtimeId,
                seatIds: selectedSeats.map((seat) => seat.seatId),
            });

            const unavailableSeats = response.data.unavailableSeats;

            if (unavailableSeats.length > 0) {
                alert(`Những ghế sau đã được đặt: ${unavailableSeats.join(', ')}. Vui lòng chọn lại!`);
                setSeats((prevSeats) =>
                    prevSeats.map((seat) =>
                        unavailableSeats.includes(seat.seatId) ? { ...seat, isAvailable: false } : seat
                    )
                );
                setSelectedSeats((prevSelected) =>
                    prevSelected.filter((seat) => !unavailableSeats.includes(seat.seatId))
                );
            } else {
                // Chuyển đến trang đặt hàng nếu tất cả ghế đều khả dụng
                navigate('/order', {
                    state: {
                        selectedSeats,
                        movieId,
                        showtime,
                    },
                });
            }
        } catch (error) {
            console.error('Error checking seat availability:', error);
            alert('Đã xảy ra lỗi khi kiểm tra trạng thái ghế. Vui lòng thử lại!');
        } finally {
            setIsChecking(false);
        }
    };

    return (
        <div className="ticket-booking">
            <h1>Đặt Vé Cho Phim</h1>
            <h2>
                Suất Chiếu:{' '}
                {showtime &&
                    `${new Date(showtime.showDate).toLocaleDateString()} ${showtime.showHour.slice(0, 5)}`}
            </h2>
            <div className="theater-layout">
                <h2>Chọn Ghế</h2>
                {theaterInfo ? (
                    <SeatMap
                        rows={theaterInfo.rows}
                        columns={theaterInfo.columns}
                        seats={seats}
                        toggleSeatSelection={toggleSeatSelection}
                        selectedSeats={selectedSeats}
                    />
                ) : (
                    <p>Đang tải thông tin rạp...</p>
                )}
                <div className="seat-legend">
                    <span className="legend-item available">Ghế trống</span>
                    <span className="legend-item selected">Ghế đang chọn</span>
                    <span className="legend-item booked">Ghế đã đặt</span>
                </div>
            </div>
            <h3>Tổng Tiền: {selectedSeats.length * seatPrice} VND</h3>
            <button onClick={handleProceedToOrder} disabled={isChecking}>
                {isChecking ? 'Đang kiểm tra...' : 'Tiếp Tục'}
            </button>
        </div>
    );
}

export default TicketBooking;
