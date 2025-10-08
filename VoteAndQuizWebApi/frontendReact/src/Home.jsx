import React, { useState, useEffect } from 'react';
import think from "./assets/voteAndQuizThink.jpeg";

// Arrays with facts for quizzes and votes
const quizFacts = [
    "The Word 'Quiz' Was a Prank? Legend says that in 1791, Richard Daly made a bet that he could invent a new word and make it widely known in 48 hours. He wrote 'QUIZ' all over the city, and by the next day, people were using it!",
    "The First TV Quiz Show Aired in 1938. The first-ever televised quiz show, 'Spelling Bee,' aired on the BBC in 1938.",
    "Pub Quizzes Started in the 1970s. Pub quizzes became popular in the UK during the 1970s when Burns & Porter started organizing them.",
    "Who Wants to Be a Millionaire? changed TV quizzes forever when it aired in 1998, introducing lifelines like 'Phone a Friend' and '50:50'.",
    "Quizzes Were Used in Ancient Greece. Philosophers like Socrates used a questioning method to challenge students' thinking."
];

const voteFacts = [
    "The First Recorded Vote Took Place in Ancient Greece in 507 BC. Citizens in Athens voted on important decisions such as whether to go to war.",
    "The First Secret Ballot Was Introduced in Australia in 1856. Before that, votes were cast openly, making it easier to intimidate or coerce voters.",
    "The Voting Age Was Lowered to 18 in the United States in 1971. The 26th Amendment gave young people the right to vote, largely due to the Vietnam War protests.",
    "The First Woman to Vote in the United States Was Susan B. Anthony in 1872. She was arrested for casting an illegal vote before women were granted the right to vote in 1920.",
    "In Switzerland, Voting Was Not Allowed for Women Until 1971. Swiss women gained the right to vote much later than in most other countries."
];

const Home = () => {
    const [currentQuizFact, setCurrentQuizFact] = useState(0);
    const [currentVoteFact, setCurrentVoteFact] = useState(0);

    useEffect(() => {
        const interval = setInterval(() => {
            // Update both quiz and vote facts at the same time
            setCurrentQuizFact((prevFact) => (prevFact + 1) % quizFacts.length);
            setCurrentVoteFact((prevFact) => (prevFact + 1) % voteFacts.length);
        }, 10000); // Change both quiz and vote facts every 10 seconds

        return () => clearInterval(interval); // Clear interval on component unmount
    }, []);

    return (
        <>
         <div className="flower-design">
            <img src="src\assets\flower.webp" alt="" />
        </div>
        <div className="homePage-container">
            <div className="content-container">
                {/* Quiz facts container on the left */}
                <div className="facts-container quiz-facts">
                    <div className="fact fade">
                        {quizFacts[currentQuizFact]}
                    </div>
                </div>

                {/* Image container in the middle */}
                <div className="image-container">
                    <img src={think} alt="Thinking" className="home-image" />
                </div>

                {/* Vote facts container on the right */}
                <div className="facts-container vote-facts">
                    <div className="fact fade">
                        {voteFacts[currentVoteFact]}
                    </div>
                </div>
            </div>

            <div className="home-container">
                <h2>The Story of V&Qs</h2>
                <p>
                    Long before the launch of the V&Qs website, the tradition of making votes and quizzes was already deeply embedded within the community. 
                    It all began years ago, in a small town where residents gathered every weekend in the local square. They engaged in lively debates, 
                    cast their votes on important local matters, and tested each other’s knowledge through quizzes. These gatherings weren't 
                    just a pastime; they were a cherished tradition that fostered unity, learning, and a sense of belonging.
                </p>
                <p>
                    As time passed, the tradition grew beyond the borders of the town. Nearby communities started participating, bringing their own 
                    topics to vote on and quizzes to share. The gatherings became larger, more diverse, and increasingly difficult to manage. 
                    But the spirit remained the same—everyone had a voice, and every voice mattered.
                </p>
                <p>
                    Recognizing the potential of this growing tradition, a brave man named Iskren decided to bring it to the digital age. 
                    He envisioned a platform where the essence of these gatherings could be preserved but made more accessible to everyone, 
                    regardless of their location. That’s how the idea of V&Qs was born.
                </p>
                <p>
                    The V&Qs website was designed to be more than just a platform. It was a tribute to the community’s roots.
                </p>
                <p>
                    Since its launch, V&Qs has become a hub for people from all walks of life. The website has not only preserved the community's 
                    tradition but has also expanded it to a global audience. Today, V&Qs is where the past meets the future, where everyone still 
                    has a voice, and where the love for knowledge continues to grow.
                </p>
                <p>
                    The story of V&Qs is a testament to the power of community and tradition, showing that with a little innovation, even the most 
                    cherished customs can evolve and thrive in the modern world.
                </p>
            </div>
        </div>
        </>
    );
};

export default Home;
