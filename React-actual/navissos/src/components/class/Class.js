import { useNavigate } from 'react-router-dom';
import React, { useEffect, useState } from 'react';
import './Class.css';

function Class() {
    const [classData, setClassData] = useState([]);
    
    useEffect(() => {
        fetch('http://localhost:5236/api/Class/GetAllClasses')
            .then(response => response.json())
            .then(data => setClassData(data));
    }, []);
    

    
    return (
        <div className='classFrame'>
            <div className='classes-data'>
                <h1 className='i-c'>All classes: </h1> 
                {classData.map(classData => (
                        <div style={{
                            backgroundColor: '#ffdbac',
                            backgroundSize: 'cover',
                            width: '200px',
                            height: '100px', 
                            border: 'none',
                            cursor: 'pointer',
                            margin: '3rem'
                        }}>{classData.id} - {classData.properties.name}</div>
                    ))}
            </div>
        </div>
    );
}

export default Class;
