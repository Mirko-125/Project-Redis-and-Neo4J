import React, { useEffect, useState } from 'react';
import './Class.css';
import '../../styling/CrudContainer.css';

const Class = () => {
    const [classOldName, setClassOldName] = useState('');
    const [className, setClassName] = useState('');
    const [classData, setClassData] = useState([]);
    const [baseStrength, setBaseStrength] = useState(0);
    const [baseAgility, setBaseAgility] = useState(0);
    const [baseIntelligence, setBaseIntelligence] = useState(0);
    const [baseStamina, setBaseStamina] = useState(0);
    const [baseFaith, setBaseFaith] = useState(0);
    const [baseExperience, setBaseExperience] = useState(0);
    const [baseLevel, setBaseLevel] = useState(0);
    const [levelGainStrength, setLevelGainStrength] = useState(0);
    const [levelGainAgility, setLevelGainAgility] = useState(0);
    const [levelGainIntelligence, setLevelGainIntelligence] = useState(0);
    const [levelGainStamina, setLevelGainStamina] = useState(0);
    const [levelGainFaith, setLevelGainFaith] = useState(0);
    const [levelGainExperience, setLevelGainExperience] = useState(0);
    const [levelGainLevel, setLevelGainLevel] = useState(0);
    const [abilityName, setAbilityName] = useState('');
    const [level, setAbilityLevel] = useState(0);


    const handleDeleteClass = () => 
    {
        fetch(`http://localhost:5236/api/Class?className=${className}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
                console.log(data);
            })
            .catch(error => {

                console.error(error);
            });
    };

    const handleAssignAbility = () => {
        const data = {
            className,
            abilityName,
            level
        }
        const dataJson = JSON.stringify(data);
        console.log(dataJson);
        
        fetch(`http://localhost:5236/api/Class/CreateAbilityPermissions`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(dataJson)
        })  
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    }


    const handleEditClass = () => {
        const classData = {
            name: className,
            baseAttributes: {
                strength: baseStrength,
                agility: baseAgility,
                intelligence: baseIntelligence,
                stamina: baseStamina,
                faith: baseFaith,
                experience: baseExperience,
                level: baseLevel
            },
            levelGainAttributes: {
                strength: levelGainStrength,
                agility: levelGainAgility,
                intelligence: levelGainIntelligence,
                stamina: levelGainStamina,
                faith: levelGainFaith,
                experience: levelGainExperience,
                level: levelGainLevel
            },
            oldName: classOldName
        };
        fetch(`http://localhost:5236/api/Class/Update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(classData)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleCreateClass = () => {
        const classData = {
            name: className,
            baseAttributes: {
                strength: baseStrength,
                agility: baseAgility,
                intelligence: baseIntelligence,
                stamina: baseStamina,
                faith: baseFaith,
                experience: baseExperience,
                level: baseLevel
            },
            levelGainAttributes: {
                strength: levelGainStrength,
                agility: levelGainAgility,
                intelligence: levelGainIntelligence,
                stamina: levelGainStamina,
                faith: levelGainFaith,
                experience: levelGainExperience,
                level: levelGainLevel
            }
        };

        fetch('http://localhost:5236/api/Class', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(classData)
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    useEffect(() => {
        fetch('http://localhost:5236/api/Class/GetAll')
            .then(response => response.json())
            .then(data => setClassData(data));
    }, []);

    return (
    <div className='class-wrap'>
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
                        margin: '3rem',
                        color: 'black'
                    }}>{classData.name}</div>
                ))}
            </div>
            <div className='crud-container'>
                <div>
                    <h1 className='i-c'>Admin: Create a new playable class</h1>
                    <div className='input-container'>
                        <input
                            className='create-input'
                            type="text"
                            placeholder="Enter name"
                            onChange={(e) => setClassName(e.target.value)}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base strength"
                            onChange={(e) => setBaseStrength(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base agility"
                            onChange={(e) => setBaseAgility(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base intelligence"
                            onChange={(e) => setBaseIntelligence(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base stamina"
                            onChange={(e) => setBaseStamina(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base faith"
                            onChange={(e) => setBaseFaith(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base experience"
                            onChange={(e) => setBaseExperience(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base level"
                            onChange={(e) => setBaseLevel(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain strength"
                            onChange={(e) => setLevelGainStrength(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain agility"
                            onChange={(e) => setLevelGainAgility(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain intelligence"
                            onChange={(e) => setLevelGainIntelligence(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain stamina"
                            onChange={(e) => setLevelGainStamina(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain faith"
                            onChange={(e) => setLevelGainFaith(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain experience"
                            onChange={(e) => setLevelGainExperience(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain level"
                            onChange={(e) => setLevelGainLevel(parseInt(e.target.value))}
                        />
                        <button className='green-bg' onClick={handleCreateClass}>Create class</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-c'>Admin: Edit a new playable class</h1>
                    <div className='input-container'>
                        <input
                            className='create-input'
                            type="text"
                            placeholder="Enter class' old name"
                            onChange={(e) => setClassOldName(e.target.value)}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base strength"
                            onChange={(e) => setBaseStrength(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base agility"
                            onChange={(e) => setBaseAgility(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base intelligence"
                            onChange={(e) => setBaseIntelligence(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base stamina"
                            onChange={(e) => setBaseStamina(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base faith"
                            onChange={(e) => setBaseFaith(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base experience"
                            onChange={(e) => setBaseExperience(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter base level"
                            onChange={(e) => setBaseLevel(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain strength"
                            onChange={(e) => setLevelGainStrength(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain agility"
                            onChange={(e) => setLevelGainAgility(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain intelligence"
                            onChange={(e) => setLevelGainIntelligence(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain stamina"
                            onChange={(e) => setLevelGainStamina(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain faith"
                            onChange={(e) => setLevelGainFaith(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain experience"
                            onChange={(e) => setLevelGainExperience(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="number"
                            placeholder="Enter level gain level"
                            onChange={(e) => setLevelGainLevel(parseInt(e.target.value))}
                        />
                        <input
                            className='create-input'
                            type="text"
                            placeholder="Enter class' new name"
                            onChange={(e) => setClassName(e.target.value)}
                        />
                        <button className='violet-bg' onClick={handleEditClass}>Edit class</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-n'>Admin: Delete a class</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Enter class name" onChange={e => setClassName(e.target.value)} />
                        <button className='red-bg' onClick={handleDeleteClass}>Delete class</button>
                    </div>
                    <h1 className='i-n'>Admin: Assign Ability</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Enter class name" onChange={e => setClassName(e.target.value)} />
                        <input type="text" placeholder="Enter ability name" onChange={e => setAbilityName(e.target.value)} />
                        <input type="number" placeholder="Enter level" onChange={e => setAbilityLevel(e.target.value)} />
                        <button className='blue-bg' onClick={handleAssignAbility}>Assign Ability</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    );
}

export default Class;
