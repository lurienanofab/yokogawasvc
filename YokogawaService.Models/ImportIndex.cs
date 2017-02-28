namespace YokogawaService.Models
{
    public class ImportIndex
    {
        private static volatile ImportIndex instance;
        private static object syncRoot = new object();

        public static ImportIndex Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ImportIndex();
                    }
                }

                return instance;
            }
        }

        public ImportIndex()
        {
            Value = -1;
        }

        public int Value { get; set; }

        public void Increment()
        {
            Value += 1;
        }
    }
}
