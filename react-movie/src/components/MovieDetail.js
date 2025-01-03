import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getMovieById, getShowtimesByMovieId } from '../services/apiService';
import './css/MovieDetail.css';

function MovieDetail() {
    const { movieId } = useParams();
    const navigate = useNavigate();
    const [movie, setMovie] = useState(null);
    const [showtimes, setShowtimes] = useState([]);
    const [selectedDate, setSelectedDate] = useState(null);
    const [selectedShowtime, setSelectedShowtime] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (movieId) {
            // Lấy dữ liệu phim
            getMovieById(movieId)
                .then(response => {
                    setMovie(response.data);
                    setError(null);
                    setLoading(false);
                })
                .catch(err => {
                    console.error('Error fetching movie details:', err);
                    setError('Failed to fetch movie details');
                    setLoading(false);
                });

            // Lấy dữ liệu giờ chiếu liên quan đến phim
            getShowtimesByMovieId(movieId)
                .then(response => {
                    setShowtimes(response.data);
                })
                .catch(err => {
                    console.error('Error fetching showtimes:', err);
                });
        } else {
            setError('Invalid movie ID');
            setLoading(false);
        }
    }, [movieId]);

    // Lấy danh sách các ngày chiếu duy nhất
    const uniqueDates = Array.from(new Set(showtimes.map(showtime => new Date(showtime.showDate).toLocaleDateString())));

    // Lọc giờ chiếu theo ngày đã chọn
    const filteredShowtimes = showtimes.filter(showtime =>
        selectedDate && new Date(showtime.showDate).toLocaleDateString() === selectedDate
    );

    const handleDateSelect = (date) => {
        setSelectedDate(date);
        setSelectedShowtime(null); // Reset giờ chiếu khi thay đổi ngày chiếu
    };

    const handleShowtimeSelect = (showtime) => {
        setSelectedShowtime(showtime);
    };

    const handleBookTicket = () => {
        if (selectedShowtime) {
            navigate(`/ticketbooking`, { state: { movieId: movie.movieId, showtime: selectedShowtime } });
        }
    };


    if (loading) {
        return <div className="loading">Loading...</div>;
    }

    if (error) {
        return <div className="error-message">{error}</div>;
    }

    if (!movie) {
        return <div className="no-details">Không tìm thấy chi tiết phim</div>;
    }

    return (
        <div className="movie-detail">
            <div className="movie-info">
                <h2 className="movie-title">{movie.title}</h2>
                <img src={movie.imageUrl} alt={`${movie.title} cover`} className="movie-image" />
                <p><strong></strong> {movie.description}</p>
                <p><strong>Thể loại:</strong> {movie.genre}</p>
                <p><strong>Thời lượng:</strong> {movie.duration} minutes</p>
                <p><strong>Ngày phát hành:</strong> {new Date(movie.releaseDate).toLocaleDateString()}</p>
            </div>


            <div className="allbutton">
                {/* Chọn ngày chiếu */}
                <div className="date-selection">
                    <label><strong>Chọn lịch chiếu:</strong></label>
                    <div className="date-boxes">
                        {uniqueDates.map(date => (
                            <button
                                key={date}
                                className={`date-box ${selectedDate === date ? 'selected' : ''}`}
                                onClick={() => handleDateSelect(date)}
                            >
                                {date}
                            </button>
                        ))}
                    </div>
                </div>
                {/* Hiển thị giờ chiếu của ngày đã chọn */}
                {selectedDate && (
                    <div className="showtime-list">
                        <label><strong>Suất chiếu trong ngày {selectedDate}:</strong></label>
                        <div className="showtime-boxes">
                            {filteredShowtimes.length > 0 ? (
                                filteredShowtimes.map(showtime => (
                                    <button
                                        key={showtime.showtimeId}
                                        className={`showtime-box ${selectedShowtime === showtime ? 'selected' : ''}`}
                                        onClick={() => handleShowtimeSelect(showtime)}
                                    >
                                        {showtime.showHour.slice(0, 5)} {/* Chỉ hiển thị giờ */}
                                    </button>
                                ))
                            ) : (
                                <p>Không có lịch chiếu hiện hoạt</p>
                            )}
                        </div>
                    </div>
                )}

                <button
                    onClick={handleBookTicket}
                    disabled={!selectedShowtime}
                    className={`book-button ${selectedShowtime ? 'active' : ''}`}
                >
                    Đặt vé
                </button>
            </div>
        </div>
    );
}

export default MovieDetail;


