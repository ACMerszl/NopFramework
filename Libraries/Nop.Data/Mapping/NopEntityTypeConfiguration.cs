using System.Data.Entity.ModelConfiguration;

namespace Nop.Data.Mapping
{
    /// <summary>
    /// ����ʵ�������ݿ��ӳ��������Ҫ�̳��Ը��ࡣ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NopEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        protected NopEntityTypeConfiguration()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// ������Ա�������Զ���ֲ�������д�˷���
        ///���һЩ�Զ����ʼ�����뵽���캯��
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}