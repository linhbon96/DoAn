import React, { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './css/MovieList.css';

function MovieList() {
    const [movies, setMovies] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [currentBannerIndex, setCurrentBannerIndex] = useState(0);
    const navigate = useNavigate();
    const titleRefs = useRef([]);

    useEffect(() => {
        axios.get('http://localhost:5175/api/Movie')
            .then(response => {
                const sortedMovies = response.data.sort((a, b) => new Date(b.releaseDate) - new Date(a.releaseDate)); // Sắp xếp theo ngày phát hành mới nhất
                setMovies(sortedMovies);
                setLoading(false);
            })
            .catch(error => {
                console.error('Error fetching movies:', error);
                setError('Failed to fetch movies');
                setLoading(false);
            });
    }, []);

    useEffect(() => {
        const interval = setInterval(() => {
            setCurrentBannerIndex((prevIndex) => (prevIndex + 1) % Math.min(6, movies.length));
        }, 5000); // Chuyển banner sau 5 giây

        return () => clearInterval(interval);
    }, [movies.length]);

    useEffect(() => {
        titleRefs.current.forEach((titleRef) => {
            if (titleRef && titleRef.textContent.length > 18) {
                titleRef.classList.add('marquee');
            } else {
                titleRef.classList.remove('marquee');
            }
        });
    }, [movies]);

    const handleBannerNext = () => {
        setCurrentBannerIndex((prevIndex) => (prevIndex + 1) % Math.min(6, movies.length)); // Quay lại phim đầu tiên
    };

    const handleBannerPrev = () => {
        setCurrentBannerIndex((prevIndex) => (prevIndex - 1 + Math.min(6, movies.length)) % Math.min(6, movies.length)); // Quay về phim cuối
    };

    const handleMovieClick = (movieId) => {
        navigate(`/movie/${movieId}`);
    };

    const handleBuyTicket = (e, movieId) => {
        e.stopPropagation();
        navigate(`/movie/${movieId}`);
    };

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    const bannerMovies = movies.slice(0, 6); // Lấy 6 phim đầu tiên cho banner

    return (
        <div className="movie-list-container">

            {/* Banner Section */}
            {bannerMovies.length > 0 && (
                <div className="banner-container">
                    <button className="banner-control prev" onClick={handleBannerPrev}>❮</button>
                    <div
                        className="movie-banner"
                        style={{
                            backgroundImage: `url(${bannerMovies[currentBannerIndex].imageUrl})`,
                            transition: 'background-image 0.5s ease-in-out' // Hiệu ứng chuyển mượt mà
                        }}
                        onClick={() => handleMovieClick(bannerMovies[currentBannerIndex].movieId)}
                    >
                        <div className="banner-content">
                            <h1>{bannerMovies[currentBannerIndex].title}</h1>
                            <p>{bannerMovies[currentBannerIndex].description}</p>
                            <button
                                onClick={(e) => handleBuyTicket(e, bannerMovies[currentBannerIndex].movieId)}
                                className="buy-ticket-button"
                            >
                                Đặt vé ngay
                            </button>
                        </div>
                    </div>
                    <button className="banner-control next" onClick={handleBannerNext}>❯</button>
                </div>
            )}
            <h1 className="h1">Phim Đang Chiếu</h1>
            {/* Movie List Section */}
            <div className="movie-list">

                <div className="movie-container">
                    <div className="movie-grid">
                        {movies.slice(0, 8).map((movie, index) => (
                            <div
                                key={movie.movieId}
                                className="movie-card"
                                onClick={() => handleMovieClick(movie.movieId)}
                            >
                                <div className="movie-rank">
                                    <span>{index + 1}</span>
                                </div>
                                <img src={movie.imageUrl} alt={`${movie.title} cover`} className="movie-cover" />
                                <h3 ref={(el) => (titleRefs.current[index] = el)}>{movie.title}</h3>
                                <p><strong>Thể loại:</strong> {movie.genre}</p>
                                <p><strong>Thời lượng:</strong> {movie.duration}</p>
                                <p><strong>Khởi chiếu:</strong> {new Date(movie.releaseDate).toLocaleDateString()}</p>
                                <button
                                    onClick={(e) => handleBuyTicket(e, movie.movieId)}
                                    className="buy-ticket-button"
                                >
                                    Đặt vé
                                </button>
                            </div>
                        ))}
                    </div>
                </div>
            </div>

        </div>
    );
}

export default MovieList;

