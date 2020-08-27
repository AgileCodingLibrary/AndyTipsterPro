Select u.Id, u.Email, u.PasswordHash, subs.Description, subs.State, subs.StartDate,subs.ExpiryDate from AspNetUsers as u
join UserSubscriptions as subs on subs.UserId = u.Id
where u.Email = 'macadixon1995@gmail.com'

--update AspNetUsers
--set PasswordHash = 'AQAAAAEAACcQAAAAEP42gJN3hON7ESkMd+NbkyfIgJGORAxgzOhdqZDQcDdZQ+VjLArSzA7TSx3YCb2BOQ=='   --general
--where email = 'macadixon1995@gmail.com'



--update AspNetUsers
--set PasswordHash = 'AQAAAAEAACcQAAAAEIpRZhW4CS8Z86g0NwfkOS1Lfyq0Nt/1z06IbY1YmyJRjfV5cq/Iusyic+Tgvms0OQ=='   --USER
--where email = 'macadixon1995@gmail.com'


--select *   from UserSubscriptions where State is null