import React from 'react';

function About() {
    return (
        <>
         <div className="flower-design">
            <img src="src\assets\flower.webp" alt="" />
        </div>
        <div className='about-container'>
            <h2>About V&Qs</h2>
            <p>This web application is designed for voting and quizzes. It uses ASP.NET Core Web Api for the backend and React for the frontend.</p>
            <p>The app has also implemented Repository pattern, it uses DTOs and default ASP.NET Core Identity for authentication</p>
        </div>
        </>
    );
}

export default About;