namespace DakarRally.DAL
{
    public abstract class BaseRepository
    {
        public Model.DakarRallyEntities GetContext()
        {
            var model = new Model.DakarRallyEntities();
            model.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            return model;
        }
    }
}