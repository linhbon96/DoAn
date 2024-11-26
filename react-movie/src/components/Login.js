import React, { useState } from 'react';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom';
import './css/LoginPage.css'; // Giả sử có file CSS riêng để định dạng giao diện

const LoginPage = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleLogin = async (e) => {
        e.preventDefault(); // Ngăn hành động submit mặc định của form

        try {
            const response = await fetch('http://localhost:5175/api/Users/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password }),
            });

            if (response.ok) {
                const data = await response.json();
                login(data.role, data.userId); // Lưu role và userId vào AuthContext
                alert('Đăng nhập thành công!');
                navigate('/'); // Chuyển hướng về trang chủ
            } else {
                const errorData = await response.json();
                setError(errorData.message || 'Tên đăng nhập hoặc mật khẩu không chính xác!');
            }
        } catch (error) {
            console.error('Error during login:', error);
            setError('Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại!');
        }
    };

    return (
        <div className="auth-container">
            <h2>Đăng Nhập</h2>
            <form onSubmit={handleLogin}>
                <div className="form-group">
                    <label htmlFor="username">Tên đăng nhập</label>
                    <input
                        id="username"
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        placeholder="Nhập tên đăng nhập"
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="password">Mật khẩu</label>
                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Nhập mật khẩu"
                        required
                    />
                </div>
                {error && <p className="error">{error}</p>} {/* Hiển thị lỗi nếu có */}
                <button type="submit">Đăng Nhập</button>
            </form>
            <p>
                Chưa có tài khoản? <a href="/register">Đăng ký</a>
            </p>
        </div>
    );
};

export default LoginPage;
