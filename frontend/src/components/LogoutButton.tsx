
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function LogoutButton() {
    const navigate = useNavigate();
    const { setIsAuthenticated } = useAuth();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const handleLogout = async () => {
        setLoading(true);
        setError("");

        try {
            const response = await fetch(
                `${API_BASE_URL}/api/auth/logout`,
                {
                    method: "POST",
                    credentials: "include"
                }
            );

            if (!response.ok) {
                throw new Error("Logout failed. Please try again.");
            }

            setIsAuthenticated(false);
            navigate("/login", { replace: true });
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : "Something went wrong."
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <button onClick={handleLogout} disabled={loading}>
                {loading ? "Logging out..." : "Logout"}
            </button>

            {error && <p role="alert">{error}</p>}
        </div>
    );
}

export default LogoutButton;