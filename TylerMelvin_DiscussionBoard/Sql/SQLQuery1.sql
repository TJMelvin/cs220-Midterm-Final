INSERT INTO Posts (DiscussionThreadId, ApplicationUserId, Title, Content, CreatedAt, IsDeleted, Timestamp)
VALUES (
  1,
  (SELECT TOP 1 Id FROM AspNetUsers),
  'Re: First Post!',
  'Congratulations on coming in first!',
  GETDATE(),
  0,
  1
);

INSERT INTO Posts (DiscussionThreadId, ApplicationUserId, Title, Content, CreatedAt, IsDeleted, Timestamp)
VALUES (
  1,
  (SELECT TOP 1 Id FROM AspNetUsers),
  'Congrats!',
  'Awesome job!',
  GETDATE(),
  0,
  2
);