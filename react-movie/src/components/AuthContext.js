import React, { createContext, useState, useContext, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const navigate = useNavigate();

    // State lưu trữ thông tin người dùng
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [userId, setUserId] = useState(null);

    // Đọc thông tin từ localStorage khi ứng dụng khởi động
    useEffect(() => {
        const storedIsLoggedIn = localStorage.getItem('isLoggedIn') === 'true';
        const storedUserRole = localStorage.getItem('userRole');
        const storedUserId = localStorage.getItem('userId');

        if (storedIsLoggedIn && storedUserRole && storedUserId) {
            setIsLoggedIn(true);
            setUserRole(storedUserRole);
            setUserId(parseInt(storedUserId, 10)); // Chuyển đổi userId từ string sang number
        }
    }, []);

    // Hàm xử lý đăng nhập
    const login = (role, id) => {
        setIsLoggedIn(true);
        setUserRole(role);
        setUserId(id);

        localStorage.setItem('isLoggedIn', 'true');
        localStorage.setItem('userRole', role);
        localStorage.setItem('userId', id.toString()); // Chuyển userId thành string để lưu trữ
    };

    // Hàm xử lý đăng xuất
    const logout = () => {
        setIsLoggedIn(false);
        setUserRole(null);
        setUserId(null);

        localStorage.removeItem('isLoggedIn');
        localStorage.removeItem('userRole');
        localStorage.removeItem('userId');

        navigate('/login'); // Điều hướng về trang đăng nhập
    };

    // Kiểm tra người dùng có phải là Admin
    const isAdmin = () => userRole === 'Admin';

    return (
        <AuthContext.Provider value={{ isLoggedIn, userRole, userId, login, logout, isAdmin }}>
            {children}
        </AuthContext.Provider>
    );
};

// Custom hook để sử dụng AuthContext dễ dàng hơn
export const useAuth = () => useContext(AuthContext);
