import React from 'react';
import { useNavigate } from 'react-router-dom';
import './TitleMenu.css';

function TitleMenu() 
{
  let navigate = useNavigate();

  return (
    <div className="wrap-map">  
      <div className='title-frame'>
        <h1 className='menu-title'>The Elder Scrolls: Navissos</h1>
        <button className='select-button' onClick={() => navigate('/player')}>Player</button>
        <button className='select-button' onClick={() => navigate('/npcs')}>NPCs</button>
        <button className='select-button' onClick={() => navigate('/monster')}>Monsters</button>
        <button className='select-button' onClick={() => navigate('/item')}>Items</button>
        <button className='select-button' onClick={() => navigate('/ability')}>Abilities</button>
        <button className='select-button' onClick={() => navigate('/class')}>Classes</button>
        <button className='select-button' onClick={() => navigate('/achievement')}>Achievements</button>
        <button className='select-button' onClick={() => navigate('/marketplace')}>Marketplace</button>
        <button className='select-button' onClick={() => navigate('/trade')}>Trade</button>
        <button className='select-button' onClick={() => navigate('/')}>Logout</button>
      </div>
    </div>
  );
}

export default TitleMenu;