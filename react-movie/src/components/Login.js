import React, { useState } from 'react';
import { useAuth } from './AuthContext';
import { useNavigate } from 'react-router-dom';
import './css/LoginPage.css';
import { loginUser } from '../services/apiService';

const LoginPage = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleLogin = async (e) => {
        e.preventDefault(); // Ngăn hành động submit mặc định của form

        try {
            const response = await loginUser({ username, password });

            if (response.status === 200) {
                const data = response.data;
                login(data.role, data.userId); // Lưu role và userId vào AuthContext
                alert('Đăng nhập thành công!');
                navigate('/'); // Chuyển hướng về trang chủ
            } else {
                setError(response.data.message || 'Tên đăng nhập hoặc mật khẩu không chính xác!');
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

