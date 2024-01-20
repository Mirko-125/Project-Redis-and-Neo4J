import React, { useEffect, useState } from 'react';
import './Achievement.css';

const Achievement = () => {
    const [achievement, setAchievement] = useState([]);
    useEffect(() => {
        fetch('http://localhost:5236/api/Achievement/AllAchievements') 
            .then(response => response.json())
            .then(data => setAchievement(data));
    }, []);
    
    return (
        <div>
            <h1 className='i-a'>Achievements</h1>
            <div className='quests'>
                {achievement.map(achievement => (
                    <button key={achievement.id} 
                    style={{
                        backgroundColor: '#8B0000',
                        backgroundSize: 'cover',
                        width: '200px',
                        height: '100px', 
                        border: 'none',
                        cursor: 'pointer',
                        margin: '3rem',
                        color: 'white'
                    }}
                    >
                        Id: [{achievement.id}]<br/>
                        {achievement.properties.name}<br/>
                        Desc: {achievement.properties.conditions}<br/>
                        Reward: {achievement.properties.points}<br/>
                        Type: {achievement.properties.type}<br/>
                    </button>
                ))}
            </div>
        </div>
    );
};

export default Achievement;