import React, { useState } from 'react';
import axios from 'axios';
import './QuizList.css';

const QuizList = () => {
    const [quizzes, setQuizzes] = useState([]);
    const [error, setError] = useState(null);
    const [showQuizzes, setShowQuizzes] = useState(false);

    const fetchQuizzes = () => {
        axios.get('https://localhost:7055/api/Quizzes', { withCredentials: true })
            .then(response => {
                setQuizzes(response.data);
                setShowQuizzes(true);
            })
            .catch(err => {
                setError(err.message);
                setShowQuizzes(true);
            });
    };

    return (
        <>
         <div className="flower-design">
            <img src="src\assets\flower.webp" alt="" />
        </div>
        <div className='quiz-list-container'>
            <h1>Quizzes</h1>
            <button className='show-quizes-btn' onClick={fetchQuizzes}>Show Quizzes</button>
            {showQuizzes && (
                <div>
                    {/* {error && <p className="error-message">Error: {error}</p>} */}
                    {error && <p className="error-message">Error: Log in to see the Quizzes!</p>}
                    <ul className='quiz-list'>
                        {quizzes.map(quiz => (
                            <li key={quiz.id} className="balloon">{quiz.name}</li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
        </>
    );
};

export default QuizList;