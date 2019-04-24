function onPhysicsComponentReady(self, msg)  
    self:FireEventServerSide{args = 'physicsReady'} 
end