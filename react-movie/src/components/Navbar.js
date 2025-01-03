import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { searchMovies } from '../services/apiService';
import { useAuth } from './AuthContext';
import './css/Navbar.css';

function Navbar() {
    const navigate = useNavigate();
    const { isLoggedIn, userRole, userId, logout } = useAuth();

    // State quản lý tìm kiếm
    const [searchQuery, setSearchQuery] = useState('');
    const [suggestions, setSuggestions] = useState([]);
    const [showDropdown, setShowDropdown] = useState(false);

    // State quản lý menu thả xuống
    const [showAdminMenu, setShowAdminMenu] = useState(false);
    const [showUserMenu, setShowUserMenu] = useState(false);

    const handleHomeClick = () => {
        navigate('/');
    };

    const handleSearchChange = async (e) => {
        const query = e.target.value;
        setSearchQuery(query);

        if (query.length > 1) {
            try {
                const response = await searchMovies(query);
                setSuggestions(response.data);
                setShowDropdown(true);
            } catch (error) {
                console.error('Error fetching movie suggestions:', error);
                setSuggestions([]);
            }
        } else {
            setSuggestions([]);
            setShowDropdown(false);
        }
    };

    const handleSearchSubmit = (e) => {
        e.preventDefault();
        if (searchQuery.trim()) {
            navigate(`/movie-list?search=${searchQuery}`);
            setSearchQuery('');
            setSuggestions([]);
            setShowDropdown(false);
        }
    };

    const handleSuggestionClick = (movieId) => {
        navigate(`/movie/${movieId}`);
        setSearchQuery('');
        setSuggestions([]);
        setShowDropdown(false);
    };

    return (
        <nav className="navbar">
            <div className="navbar-left">
                <h2 onClick={handleHomeClick} style={{ cursor: 'pointer' }}>MovieBooking</h2>

                {/* Thanh tìm kiếm */}
                <div className="search-form">
                    <div className="search-input-wrapper">
                        <input
                            type="text"
                            className="search-input"
                            placeholder="Tìm kiếm phim..."
                            value={searchQuery}
                            onChange={handleSearchChange}
                            onFocus={() => setShowDropdown(suggestions.length > 0)}
                            onBlur={() => setTimeout(() => setShowDropdown(false), 200)}
                        />
                    </div>

                    {showDropdown && suggestions.length > 0 && (
                        <ul className="search-dropdown">
                            {suggestions.map((movie) => (
                                <li
                                    key={movie.movieId}
                                    onClick={() => handleSuggestionClick(movie.movieId)}
                                >
                                    <img
                                        src={movie.imageUrl}
                                        alt={movie.title}
                                        className="dropdown-image"
                                    />
                                    <span>{movie.title}</span>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            </div>

            <ul className="navbar-right">
                <li><a href="/">Trang Chủ</a></li>

                {!isLoggedIn && <li><a href="/login">Đăng Nhập</a></li>}

                {isLoggedIn && userRole === 'Admin' && (
                    <li
                        className="dropdown"
                        onMouseEnter={() => setShowAdminMenu(true)}
                        onMouseLeave={() => setShowAdminMenu(false)}
                    >
                        <a href="#">Admin</a>
                        {showAdminMenu && (
                            <ul className="dropdown-menu">
                                <li><a href="/admin">Quản Lý</a></li>
                                <li><a href="/showtime">Đặt Lịch Chiếu</a></li>
                                <li><a href="/item">Quản Lý Vật Phẩm</a></li>
                                <li><a href={`/TicketInfo/${userId}`}>Thông Tin Vé</a></li>
                                <li><a href="/theaters">Quản Lý Rạp Phım</a></li>
                                <li><a href="/sales-report">Bảng Doanh Thu</a></li>

                                <li><a href="#" onClick={logout}>Đăng Xuất</a></li>
                            </ul>
                        )}
                    </li>
                )}

                {isLoggedIn && userRole === 'User' && userId && (
                    <li
                        className="dropdown"
                        onMouseEnter={() => setShowUserMenu(true)}
                        onMouseLeave={() => setShowUserMenu(false)}
                    >
                        <a href="#">User</a>
                        {showUserMenu && (
                            <ul className="dropdown-menu">
                                <li><a href={`/TicketInfo/${userId}`}>Thông Tin Vé</a></li>
                                <li><a href="#" onClick={logout}>Đăng Xuất</a></li>
                            </ul>
                        )}
                    </li>
                )}
            </ul>
        </nav>
    );
}

export default Navbar;